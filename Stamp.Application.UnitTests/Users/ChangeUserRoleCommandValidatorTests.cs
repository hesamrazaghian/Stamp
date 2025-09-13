using System;
using System.Linq;
using FluentValidation.TestHelper;
using Shouldly;
using Stamp.Application.Commands.Users;
using Stamp.Application.Validators;
using Stamp.Domain.Enums;
using Xunit;

namespace Stamp.Application.UnitTests.Users
{
    public class ChangeUserRoleCommandValidatorTests
    {
        #region Fields

        private readonly ChangeUserRoleCommandValidator _validator;

        #endregion

        #region Constructor

        public ChangeUserRoleCommandValidatorTests( )
        {
            _validator = new ChangeUserRoleCommandValidator( );
        }

        #endregion

        #region Tests

        [Fact]
        public void Validator_Should_Pass_For_Valid_Input( )
        {
            // Arrange
            var command = new ChangeUserRoleCommand
            {
                UserId = Guid.NewGuid( ),
                NewRole = RoleEnum.Admin
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
            var command = new ChangeUserRoleCommand
            {
                UserId = Guid.Empty,
                NewRole = RoleEnum.User
            };

            // Act
            var result = _validator.TestValidate( command );

            // Assert
            result.ShouldHaveValidationErrorFor( x => x.UserId )
                  .WithErrorMessage( "UserId is required." );
        }

        [Fact]
        public void Validator_Should_Fail_When_NewRole_Is_Invalid( )
        {
            // Arrange
            var command = new ChangeUserRoleCommand
            {
                UserId = Guid.NewGuid( ),
                NewRole = ( RoleEnum )999 // invalid enum value
            };

            // Act
            var result = _validator.TestValidate( command );

            // Assert
            result.ShouldHaveValidationErrorFor( x => x.NewRole )
                  .WithErrorMessage( "Invalid role specified." );
        }

        [Fact]
        public void Validator_Should_Fail_When_Both_Fields_Are_Invalid( )
        {
            // Arrange
            var command = new ChangeUserRoleCommand
            {
                UserId = Guid.Empty,
                NewRole = ( RoleEnum )999
            };

            // Act
            var result = _validator.TestValidate( command );

            // Assert
            result.Errors.Count.ShouldBe( 2 );
            result.Errors.Any( e => e.ErrorMessage == "UserId is required." ).ShouldBeTrue( );
            result.Errors.Any( e => e.ErrorMessage == "Invalid role specified." ).ShouldBeTrue( );
        }

        #endregion
    }
}
