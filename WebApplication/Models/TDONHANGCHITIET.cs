//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TDONHANGCHITIET
    {
        public string ID { get; set; }
        public string DMATHANGID { get; set; }
        public Nullable<decimal> DONGIA { get; set; }
        public Nullable<decimal> SOLUONG { get; set; }
        public Nullable<decimal> TILEGIAMGIA { get; set; }
        public Nullable<decimal> TIENGIAMGIA { get; set; }
        public Nullable<decimal> THANHTIEN { get; set; }
        public string DKHUYENMAIID { get; set; }
    
        public virtual DKHUYENMAI DKHUYENMAI { get; set; }
        public virtual DMATHANG DMATHANG { get; set; }
    }
}