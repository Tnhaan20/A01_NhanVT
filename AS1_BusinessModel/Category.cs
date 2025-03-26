using System;
using System.Collections.Generic;

namespace AS1_BusinessModel;

public partial class Category
{
    public short CategoryId { get; set; }

    public string CategoryName { get; set; }

    public string CategoryDesciption { get; set; }

    public short? ParentCategoryId { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Category> InverseParentCategory { get; set; } = new List<Category>();

    public virtual ICollection<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();

    public virtual Category ParentCategory { get; set; }
}
