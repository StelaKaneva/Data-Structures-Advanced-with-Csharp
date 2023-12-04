using System;
using System.Collections.Generic;
using System.Linq;

namespace Exam.Categorization
{
    public class Categorizator : ICategorizator
    {
        private Dictionary<string, Category> categories = new Dictionary<string, Category>();
        public void AddCategory(Category category)
        {
            if (this.categories.ContainsKey(category.Id))
            {
                throw new ArgumentException();
            }

            category.Depth = 0;

            categories.Add(category.Id, category);
        }

        public void AssignParent(string childCategoryId, string parentCategoryId)
        {
            if (!categories.ContainsKey(childCategoryId) || !categories.ContainsKey(parentCategoryId))
            {
                throw new ArgumentException();
            }

            var childCategory = this.categories[childCategoryId];
            var parentCategory = this.categories[parentCategoryId];

            if (parentCategory.Children.Contains(childCategory))
            {
                throw new ArgumentException();
            }

            childCategory.Parent = parentCategory;
            parentCategory.Children.Add(childCategory);

            var ancestor = parentCategory;
            while (ancestor.Parent != null)
            {
                ancestor = ancestor.Parent;
            }

            UpdateParentDepth(ancestor);
        }

        private int UpdateParentDepth(Category node)
        {
            if (node == null)
            {
                return 0;
            }

            var depth = 0;

            foreach (var child in node.Children)
            {
                depth = Math.Max(depth, UpdateParentDepth(child));
            }

            node.Depth = depth + 1;

            return node.Depth;
        }

        public bool Contains(Category category)
        {
            return this.categories.ContainsKey(category.Id);
        }

        public IEnumerable<Category> GetChildren(string categoryId)
        {
            if (!categories.ContainsKey(categoryId))
            {
                throw new ArgumentException();
            }

            var children = new HashSet<Category>();

            GetAllChidren(categoryId, children);

            return children;
        }

        private void GetAllChidren(string categoryId, HashSet<Category> children)
        {
            foreach (var child in categories[categoryId].Children)
            {
                children.Add(child);
                GetAllChidren(child.Id, children);
            }
        }

        public IEnumerable<Category> GetHierarchy(string categoryId)
        {
            if (!categories.ContainsKey(categoryId))
            {
                throw new ArgumentException();
            }

            var stack = new Stack<Category>();

            GetAllParentsRecursively(categories[categoryId], stack);

            return stack;
        }

        private void GetAllParentsRecursively(Category category, Stack<Category> stack)
        {
            if (category == null)
            {
                return;
            }

            stack.Push(category);

            GetAllParentsRecursively(category.Parent, stack);
        }

        public IEnumerable<Category> GetTop3CategoriesOrderedByDepthOfChildrenThenByName()
        {
            return this.categories.Values
                            .OrderByDescending(c => c.Depth)
                            .ThenBy(c => c.Name)
                            .Take(3);
        }

        public void RemoveCategory(string categoryId)
        {
            if (!categories.ContainsKey(categoryId))
            {
                throw new ArgumentException();
            }

            var category = this.categories[categoryId];
            categories.Remove(categoryId);

            RemoveChildrenRecursively(category);

            if (category.Parent != null)
            {
                category.Parent.Children.Remove(category);
            }
        }

        private void RemoveChildrenRecursively(Category category)
        {
            foreach (var child in category.Children)
            {
                RemoveChildrenRecursively(child);
                categories.Remove(child.Id);
            }
        }

        public int Size()
            => this.categories.Count;
    }
}
