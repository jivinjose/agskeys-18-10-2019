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

    public partial class loan_track_table
    {
        public int id { get; set; }
        
        [Display(Name = "Customer Id")]
        public string loanid { get; set; }

        [Display(Name = "Employee Id")]
        public string employeeid { get; set; }

        [Display(Name = "Track Time")]
        public string tracktime { get; set; }

        [Display(Name = "Internal Comment")]
        public string internalcomment { get; set; }

        [Display(Name = "External Comment")]
        public string externalcomment { get; set; }

        [Display(Name = "Added by")]
        public string addedby { get; set; }

        [Display(Name = "Added Date")]
        public string datex { get; set; }
    }
}
