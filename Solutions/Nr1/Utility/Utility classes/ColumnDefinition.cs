using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leander.Nr1
{
    public class ColumnDefinition
    {
        public bool isInComplete;
        public bool isIncorrect;
        public bool isException;

        private string _fileNameFullPath;
        private string _row;
        private int _rowNr;
        private string _followingRows;
        private string _enumeratorDataType;
        private string _dataType;
        private string _headingName;
        private string _notes;
        private string _required;
        private string _exampleData;
        private string _getExampleParameter1;
        private string _getExampleParameter2;
        private string _getExampleParameter3;


        public ColumnDefinition(string fileNameFullPath, string[] rows, int index)
        {
            int start, end, i;
            string row, str, key, value;
            char c;
            string[] v;

            row = Utility.ReplaceCharWithAnotherCharWithinAString(rows[index], ',', ' ');

            this.isException = false;
            this._fileNameFullPath = fileNameFullPath;
            this._row = row;
            this._rowNr = 1 + index;
            this._enumeratorDataType = "NULL";
            this._dataType = "NULL";
            this._headingName = "NULL";
            this._notes = "NULL";
            this._required = "NULL";
            this._exampleData = "NULL";
            this._getExampleParameter1 = "NULL";
            this._getExampleParameter2 = "NULL";
            this._getExampleParameter3 = "NULL";

            try
            {        
                if (row.IndexOf("ColumnRequiredIf") >= 0)
                {
                    str = row.Substring(row.IndexOf("ColumnRequiredIf"));
                    start = Utility.ReturnIndexForChar(0, str, '(');
                    end = Utility.ReturnIndexForChar(0, str, ')');
                    row = row.Replace(str.Substring(start, end - start + 1), " ");
                }

                start = Utility.ReturnIndexForChar(0, row, '{');
                end = row.IndexOf(", ExampleData =");

                if ((start >= 0) && (end >= 0) && (end > start))
                {
                    isInComplete = false;
                    str = row.Substring(1 + start, end - start - 1).Trim();
                    v = str.Split(',');

                    for (i = 0; i < v.Length; i++)
                    {
                        Utility.GetKeyValueOfAssignment(v[i], out key, out value);

                        switch (key)
                        {
                            case "EnumeratorDataType":
                                this._enumeratorDataType = value;
                                break;
                            case "DataType":
                                this._dataType = value;
                                break;
                            case "HeadingName":
                                this._headingName = value;
                                break;
                            case "Required":
                                this._required = value;
                                break;
                            case "Notes":
                                _notes = value;
                                break;
                        }
                    }

                    this._exampleData = row.Substring(15 + end);
                    c = this._exampleData[this._exampleData.Length - 1];

                    while ((!char.IsLetter(c)) && (!char.IsDigit(c)) && (c != '"'))
                    {
                        this._exampleData = this._exampleData.Substring(0, this._exampleData.Length - 1);
                        c = this._exampleData[this._exampleData.Length - 1];
                    }

                    this._exampleData += ")";
                    if (this._exampleData.IndexOf("typeof") >= 0)
                        this._exampleData += ")";

                    end = this._exampleData.Length - 1;

                    start = Utility.ReturnIndexForChar(0, this._exampleData, '(');

                    if ((start == -1) || (end == -1) || (end <= start))
                        throw new Exception("Error when parse parameters to GetExample!");

                    str = this._exampleData.Substring(start + 1, end - start - 1);

                    v = str.Split(',');

                    for (i = 0; i < v.Length; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                this._getExampleParameter1 = v[0].Trim();
                                break;
                            case 1:
                                this._getExampleParameter2 = v[1].Trim();
                                break;
                            case 2:
                                this._getExampleParameter3 = v[2].Trim();
                                break;
                        }
                    }

                    this.isIncorrect = false; //Default

                    if ((this._enumeratorDataType == "NULL") || (this._dataType == "NULL") || (this._headingName == "NULL") || (this._required == "NULL") || (this._exampleData == "NULL"))
                        this.isIncorrect = true;
                    else
                    {
                        if ((this._dataType != this._getExampleParameter1) || (this._headingName != this._getExampleParameter2))
                            this.isIncorrect = true;
                        else
                        {
                            if ((_dataType == "EnumSupportedDataTypes.Enumerator") && (this._getExampleParameter3 != ("enumType: " + _enumeratorDataType)))
                                this.isIncorrect = true;
                            else if ((_dataType == "EnumSupportedDataTypes.Enumerator") && (!this._notes.StartsWith(string.Format("GetNotesForEnum({0})", this._enumeratorDataType))))
                                this.isIncorrect = true;
                        }
                    }
                }
                else
                {
                    this.isInComplete = true;
                    StringBuilder sb = new StringBuilder();
                    i = 0;
                    while ((i < 10) && ((index + 1 + i) < rows.Length))
                    {
                        sb.Append(rows[index + 1 + i] + "\r\n");
                        i++;
                    }

                    this._followingRows = sb.ToString().Trim();
                }
            }
            catch
            {
                this.isException = true;
            }

        }

        public override string ToString()
        {
            if (isInComplete)
            {
                return string.Format("FileNameFullPath: {0} (row {1})\r\n{2}", this._fileNameFullPath, this._rowNr.ToString(), this._row + "\r\n" + this._followingRows);
            }
            else
            {
                return string.Format("FileNameFullPath: {0}\r\n" +
                    "Row: {1}\r\n" +
                    "RowNr {2}\r\n" + 
                    "EnumeratorDataType: {3}\r\n" +
                    "DataTyp: {4}\r\n" +
                    "HeadingName: {5}\r\n" +
                    "Required: {6}\r\n" +
                    "Notes: {7}\r\n" +
                    "ExampleData: {8}\r\n" +
                    "ExampleParameter1: {9}\r\n" +
                    "ExampleParameter2: {10}\r\n" +
                    "ExampleParameter3: {11}", this._fileNameFullPath,
                    this._row,
                    this._rowNr,
                    this._enumeratorDataType,
                    this._dataType,
                    this._headingName,
                    this._required,
                    this._notes,
                    this._exampleData,
                    this._getExampleParameter1,
                    this._getExampleParameter2,
                    this._getExampleParameter3);
            }
        }
    }
}
