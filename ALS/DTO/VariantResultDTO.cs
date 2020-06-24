using System;
using ALS.EntityСontext;

namespace ALS.DTO
{
    public class VariantResultDTO
    {
        public int VariantNumber { get; set; }
        public DateTime AssignDateTime { get; set; }
        public DateTime? SendDate { get; set; }
        public Evaluation Evaluation { get; set; }
        public double Mark { get; set; }
    }
}