using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFitness.Helpers
{
    public static class GridHelper
    {
        public static ColumnDefinitionCollection CreateCommonColumns()
        {
            return new ColumnDefinitionCollection
            {
                new ColumnDefinition { Width = new GridLength(60) },   // SET
                new ColumnDefinition { Width = new GridLength(100) },  // WEIGHT
                new ColumnDefinition { Width = new GridLength(100) },  // REPS
                new ColumnDefinition { Width = new GridLength(60) }    // DONE
            };
        }
    }
}
