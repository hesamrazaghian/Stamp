namespace Stamp.Application.Exceptions
{
    // Exception type for invalid role assignments during user registration
    public class InvalidRoleAssignmentException : Exception
    {
        public InvalidRoleAssignmentException( string message ) : base( message ) { }
    }
}
