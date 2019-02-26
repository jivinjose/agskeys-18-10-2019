//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace agskeys.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web;
    using System.Web.Security;

    public partial class customer_profile_table
    {
        public int id { get; set; }

        [Display(Name = "User Name")]
        [Required(ErrorMessage = "This field can not be empty.")]
        public string customerid { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "This field can not be empty.")]
        public string name { get; set; }

        [Display(Name = "Phone Number")]
        [Required(ErrorMessage = "You must provide a phone number")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string phoneno { get; set; }

        [Display(Name = "Date Of Birth")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public string dob { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string email { get; set; }

        [Display(Name = "Alternate Phone Number")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string alterphoneno { get; set; }

        [Display(Name = "Photo")]
        public string profileimg { get; set; }

        public HttpPostedFileBase ImageFile { get; set; }

        [Required(ErrorMessage = "You must enter a password")]
        [MembershipPassword(
        MinRequiredNonAlphanumericCharacters = 1,
        MinNonAlphanumericCharactersError = "Your password needs to contain at least one symbol (!, @, #, etc).",
        ErrorMessage = "Your password must be 6 characters long and contain at least one symbol (!, @, #, etc).",
        MinRequiredPasswordLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string password { get; set; }

        [Display(Name = "Added By")]
        public string addedby { get; set; }

        [Display(Name = "Added Date")]
        public string datex { get; set; }

        [Display(Name = "Address")]
        [Required(ErrorMessage = "You must provide a address")]
        [DataType(DataType.MultilineText)]
        public string address { get; set; }

        [Display(Name = "Wedding Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public string weddingdate { get; set; }
    }
}
