using Application.Abstractions;
using Application.Common;
using Profiles.Application.Abstractions;
using Profiles.Application.Errors;
using Profiles.Application.Features.UpdateProfile.Models;

namespace Profiles.Application.Features.UpdateProfile
{
    public sealed class UpdateProfileHandler : IUpdateProfileHandler
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public UpdateProfileHandler(
            IProfileRepository profileRepository,
            IApplicationDbContext dbContext,
            ICurrentUserService currentUserService,
            IDateTimeProvider dateTimeProvider)
        {
            _profileRepository = profileRepository;
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Result> HandleAsync(
            UpdateProfileRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!_currentUserService.IsAuthenticated ||
                _currentUserService.UserId is null)
            {
                return Result.Failure(ProfileErrors.Unauthorized);
            }

            if (string.IsNullOrWhiteSpace(request.DisplayName) ||
                request.DisplayName.Length > 100)
            {
                return Result.Failure(ProfileErrors.InvalidDisplayName);
            }

            if (request.Bio?.Length > 500)
            {
                return Result.Failure(ProfileErrors.BioTooLong);
            }

            var profile =
                await _profileRepository.GetByUserIdAsync(
                    _currentUserService.UserId.Value,
                    cancellationToken);

            if (profile is null)
            {
                return Result.Failure(ProfileErrors.NotFound);
            }

            profile.Update(
                request.DisplayName,
                request.Bio,
                _dateTimeProvider.UtcNow);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
