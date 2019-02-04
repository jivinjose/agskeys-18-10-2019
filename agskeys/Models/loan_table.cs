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

    public partial class loan_table
    {
        public int id { get; set; }

        [Required(ErrorMessage = "You must choose customer id")]
        [Display(Name = "Customer Id")]
        public string customerid { get; set; }

        [Required(ErrorMessage = "You must choose partner")]
        [Display(Name = "Partner")]
        public string partnerid { get; set; }

        [Required(ErrorMessage = "You must choose Bank")]
        [Display(Name = "Bank")]
        public string bankid { get; set; }

        [Required(ErrorMessage = "You must choose loan type")]
        [Display(Name = "Loan Type")]
        public string loantype { get; set; }

        [Required(ErrorMessage = "please enter loan amount")]
        [Display(Name = "Loan Amount")]
        public string loanamt { get; set; }

        [Required(ErrorMessage = "please enter disbursement amount")]
        [Display(Name = "Disbursement Amount")]
        public string disbursementamt { get; set; }

        [Required(ErrorMessage = "please enter rate of intrest")]
        [Display(Name = "Disbursement Rate Of Intrest")]
        public string rateofinterest { get; set; }

        [Display(Name = "Sactioned Copy")]
        public string sactionedcopy { get; set; }

        public HttpPostedFileBase sactionedCopyFile { get; set; }

        [Display(Name = "ID Copy")]
        public string idcopy { get; set; }

        public HttpPostedFileBase idCopyFile { get; set; }

        [Display(Name = "Added By")]
        public string addedby { get; set; }

        [Display(Name = "Added Date")]
        public string datex { get; set; }
    }
}
