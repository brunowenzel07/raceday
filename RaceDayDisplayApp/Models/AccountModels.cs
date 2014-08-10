using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace RaceDayDisplayApp.Models
{
    public class UsersContext : DbContext
    {
        public UsersContext()
            : base("RaceDayDB")
        {
        }

        public DbSet<Users> UserProfiles { get; set; }
    }

    [Table("UserProfile")]
    public class Users
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public bool NoNewsletter { get; set; }

        public string SignUpComment { get; set; }

        public int UILanguageId { get; set; }
    }

    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "Label_RegistrationPage_UsernameDisplayName", ResourceType = typeof(RaceDayDisplayApp.App_GlobalResources.Labels))]
        public string UserName { get; set; }

        public string ExternalLoginData { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "Label_RegistrationPage_UsernameDisplayName", ResourceType = typeof(RaceDayDisplayApp.App_GlobalResources.Labels))]
        public string UserName { get; set; }

        //[Required]
        //public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessageResourceName = "ErrorMessage_RegistrationPage_Password_StringLength", ErrorMessageResourceType = typeof(RaceDayDisplayApp.App_GlobalResources.Labels), MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Label_RegistrationPage_PasswordDisplayName", ResourceType = typeof(RaceDayDisplayApp.App_GlobalResources.Labels))]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Label_RegistrationPage_ConfirmPasswordDisplayName", ResourceType = typeof(RaceDayDisplayApp.App_GlobalResources.Labels))]
        [Compare("Password", ErrorMessageResourceName = "ErrorMessage_RegistrationPage_ConfirmPassword_Compare", ErrorMessageResourceType = typeof(RaceDayDisplayApp.App_GlobalResources.Labels))]
        public string ConfirmPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessageResourceName = "ErrorMessage_RegistrationPage_Email_StringLength", ErrorMessageResourceType = typeof(RaceDayDisplayApp.App_GlobalResources.Labels), MinimumLength = 6)]
        [Display(Name = "Label_RegistrationPage_EmailDisplayName", ResourceType = typeof(RaceDayDisplayApp.App_GlobalResources.Labels))]
        public string Email { get; set; }

        [Display(Name = "Label_RegistrationPage_NoToNewsletterDisplayName", ResourceType = typeof(RaceDayDisplayApp.App_GlobalResources.Labels))]
        public bool NoNewsletter { get; set; }

        [Display(Name = "Label_RegistrationPage_CountriesDisplayName", ResourceType = typeof(RaceDayDisplayApp.App_GlobalResources.Labels))]
        public string[] CountryIDs { get; set; }

        public IEnumerable<Country> SelectedCountries { get; set; }

        [Display(Name = "Label_RegistrationPage_SignUpCommentDisplayName", ResourceType = typeof(RaceDayDisplayApp.App_GlobalResources.Labels))]
        public string SignUpComment { get; set; }

        [Display(Name = "Label_RegistrationPage_SelectLanguageDisplayName", ResourceType = typeof(RaceDayDisplayApp.App_GlobalResources.Labels))]
        public int UILanguageId { get; set; }

        public IEnumerable<System.Web.Mvc.SelectListItem> LanguageSelectList { get; set; }

        public IEnumerable<Country> AllCountries { get; set; }
    }

    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }
}
