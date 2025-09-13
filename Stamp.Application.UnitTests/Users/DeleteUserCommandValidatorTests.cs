using System;
using FluentValidation.TestHelper;
using Shouldly;
using Stamp.Application.Commands.Users;
using Stamp.Application.Validators;
using Xunit;

namespace Stamp.Application.UnitTests.Users
{
    public class DeleteUserCommandValidatorTests
    {
        #region Fields

        private readonly DeleteUserCommandValidator _validator;

        #endregion

        #region Constructor

        public DeleteUserCommandValidatorTests( )
        {
            _validator = new DeleteUserCommandValidator( );
        }

        #endregion

        #region Tests

        [Fact]
        public void Validator_Should_Pass_For_Valid_Input( )
        {
            // Arrange
            var command = new DeleteUserCommand
            {
                UserId = Guid.NewGuid( ),
                SoftDelete = true
            };

            // Act
            var result = _validator.TestValidate( command );

            // Assert
            result.IsValid.ShouldBeTrue( );
        }

        [Fact]
        public void Validator_Should_Fail_When_UserId_Is_Empty( )
        {
            // Arrange
            var command = new DeleteUserCommand
            {
                UserId = Guid.Empty,
                SoftDelete = false
            };

            // Act
            var result = _validator.TestValidate( command );

            // Assert
            result.ShouldHaveValidationErrorFor( x => x.UserId )
                  .WithErrorMessage( "UserId is required." );
        }

        [Fact]
        public void Validator_Should_Pass_When_SoftDelete_Is_False_And_UserId_Is_Valid( )
        {
            // Arrange
            var command = new DeleteUserCommand
            {
                UserId = Guid.NewGuid( ),
                SoftDelete = false
            };

            // Act
            var result = _validator.TestValidate( command );

            // Assert
            result.IsValid.ShouldBeTrue( );
        }

        #endregion
    }
}
