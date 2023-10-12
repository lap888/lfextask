using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Yoyo.Entity.Models.luckdraw
{
    [Table("yoyo_luckydraw_defaultuser")]
    public class Yoyo_Luckydraw_DefaultUser
    {
        [Key]
        public long UserId { get; set; }
    }
}
