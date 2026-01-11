using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFitness.Models
{
    public class ArticleData
    {
        public string PageTitle { get; set; }      
        
        public string Category { get; set; }   
        
        public string MainTitlePart1 { get; set; }   
        public string MainTitlePart2 { get; set; }   

        public string Description { get; set; }  
        
        public string KeyTip1Title { get; set; }
        public string KeyTip1Text { get; set; }

        public string KeyTip2Title { get; set; }
        public string KeyTip2Text { get; set; }

        public string AdditionalInfo { get; set; }

    }
}
