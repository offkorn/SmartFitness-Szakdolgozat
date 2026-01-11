using SmartFitness.Models; 

namespace SmartFitness.Pages
{
    public partial class ArticleDetailPage : ContentPage
    {
        public ArticleDetailPage(ArticleData data)
        {
            InitializeComponent();
            
            BindingContext = data;
        }
    }
}