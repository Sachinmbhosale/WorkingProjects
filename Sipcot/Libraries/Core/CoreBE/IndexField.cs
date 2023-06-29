using System.Collections.Generic;

namespace Lotex.EnterpriseSolutions.CoreBE
{
    public class IndexField
    {
        public IndexField()
        {
            ListItemId = 0;
            ListItemValue = string.Empty;
            Indexidentity = 0;
            IndexSubidentity = 0;
            Values = string.Empty;
            SubValues = string.Empty;

            IndexName = string.Empty;
            EntryType = string.Empty;
            EntrySubType = string.Empty;
            DataType = string.Empty;
            Mandatory = string.Empty;
            Display = string.Empty;
            MinLength = 0;
            MaxLength = 0;
            TemplateID = 0;
            IsCopied = 0;
            ActiveIndex = string.Empty;
            SortOrder = 0;
            OrginalOrder = 0;
            IndexFldId = 0;
            MainTagName = string.Empty;
            SubTagName = string.Empty;
            CharIndexDataType = string.Empty;
            haschild = string.Empty;
        }
        public int ListItemId { get; set; }
        public string ListItemValue { get; set; }
        public string IndexName { get; set; }
        public string EntryType { get; set; }
        public string EntrySubType { get; set; }
        public string DataType { get; set; }
        public int MinLength { get; set; }
        public int MaxLength { get; set; }
        //MD -Modified
        public string Mandatory { get; set; }
        public string Display { get; set; }
        public string Values { get; set; }
        public string SubValues { get; set; }
        public int Indexidentity { get; set; }
        public int IsCopied { get; set; }
        public int IndexSubidentity { get; set; }
        public List<Category> Category { get; set; }
        public char CharDataType
        {
            get
            {
                switch (DataType)
                {
                    case "String":
                        return 'v';
                    case "Integer":
                        return 'i';
                    case "DateTime":
                        return 'd';
                    case "Boolen":
                        return 'b';
                    default:
                        return ' ';
                }
            }
            set
            {
                switch (value)
                {
                    case 'v':
                        DataType = "String"; break;
                    case 'i':
                        DataType = "Integer"; break;
                    case 'd':
                        DataType = "DateTime"; break;
                    case 'b':
                        DataType = "Boolen"; break;
                    default:
                        break;
                }

            }
        }
        public int TemplateID { get; set; }
        public string ActiveIndex { get; set; }
        public int SortOrder { get; set; }
        public int OrginalOrder { get; set; }
        public int IndexFldId { get; set; }
        public string MainTagName { get; set; }
        public string SubTagName { get; set; }
        public string CharIndexDataType { get; set; }
        public string haschild { get; set; }            
    }
}
