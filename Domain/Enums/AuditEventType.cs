
namespace Domain.Enums
{
    public enum AuditEventType
    {
        UserLoggedIn = 1,
        UserLoggedOut = 2,
        UserCreated = 3,
        UserProfileUpdated = 4,
        UserPermissionsUpdated = 5,
        UserAddedToGroup = 6,
        UserRemovedFromGroup = 7,

        DocumentCreated = 20,
        DocumentDeleted = 21,
        DocumentDownloaded = 22,
        DocumentsExported = 23,

        DocumentTypeCreated = 40,

        GroupCreated = 60,
        GroupPermissionsUpdated = 61
    }
}
