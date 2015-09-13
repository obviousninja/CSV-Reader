using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using Microsoft.VisualBasic.FileIO;
using System.Text.RegularExpressions;

namespace csvReader
{
   

    //reprsents ONE row of csv data
    public class Rowdata : IComparable<Rowdata>
    {
        public static int currentIndex =0;

        //title, and content of that title field
        private Dictionary<String, String> row = new Dictionary<String, String>();

        public Rowdata()
        {
            //go through all of the titleArray elements and use the element as key for row
            for (int i = 0; i < Program.titleArray.Count; i++)
            {
                if(row.ContainsKey(Program.titleArray[i]) == false)
                 row.Add(Program.titleArray[i], null);
                
            }
        }

        public Rowdata(Rowdata data)
        {
            for (int i = 0; i < Program.titleArray.Count; i++)
            {
                row.Add(Program.titleArray[i], data.getValue(Program.titleArray[i]));
            }
        }

        public Dictionary<String, String> getDict(){
            return row;
        }
            
        public bool setValue(String key, String value)
        {
            if (Program.stringEmpty(key))
            {
                return false;
            }
            if (Program.stringEmpty(value))
            {
                return false;
            }


            row[key] = value;
           

            return true;
        }

        //return null if value doesn't exist, otherwise return the value
        public String getValue(String key)
        {
            if (String.IsNullOrEmpty(key) || String.IsNullOrWhiteSpace(key))
            {
                return null;
            }
            //Console.WriteLine("Key: " + key);
            String value = "";
            if (row.TryGetValue(key, out value))
            {
                return value;
            }
          
                return null;
            


           
        }

        public void incremCount()
        {
            currentIndex++;
        }

        //returns true if central email of current row is the same as from the data
        public bool EqualsEmail(String data)
        {
            if (isNull(data))
            {
                return false;
            }

            String temp;
            row.TryGetValue("central email", out temp);
            if (isNull(temp))
                return false;

            return temp.Equals(data);

        }

        public void writeToStream(StreamWriter stream)
        {
            /*Dictionary<String, String> curData = row;
            foreach (KeyValuePair<String, String> keyval in curData)
            {
                Console.WriteLine("FieldName: {0}, DataContent {1}", keyval.Key, keyval.Value);
                //write the datarow
                
            }*/

          /*  for (int i = 0; i < Program.titleArray.Count; i++)
            {
                stream.Write(row[Program.titleArray[i]] + ", ");
                stream.Flush();
            }*/

            stream.Write(row["central email"] + ",");
            stream.Flush();
            stream.Write(row["last name"] + ",");
            stream.Flush();

            stream.Write(row["first name"] + ",");
            stream.Flush();

            stream.Write(row["full name"] + ",");
            stream.Flush();

            stream.Write(row["company"] + ",");
            stream.Flush();

            stream.Write(row["title"] + ",");
            stream.Flush();



            stream.WriteLine();
            stream.Flush();

        }
        public void output()
        {


            Dictionary<String, String> curData = row;

                foreach (KeyValuePair<String, String> keyval in curData)
                {
                    Console.WriteLine("FieldName: {0}, DataContent {1}", keyval.Key, keyval.Value);
                }


            
        }

        // Default comparer for Rowdata type. 
        public int CompareTo(Rowdata comparePart)
        {
            // A null value means that this object is greater. 
           
           //return row["central email"].CompareTo(comparePart.getValue("central email"));
            //both null
            if ((String.IsNullOrWhiteSpace(row["central email"]) || String.IsNullOrEmpty(row["central email"])) && (String.IsNullOrWhiteSpace(comparePart.getValue("central email")) || String.IsNullOrEmpty(comparePart.getValue("central email"))))
            {
                return 0;
            }

            if ((String.IsNullOrWhiteSpace(row["central email"])==false || String.IsNullOrEmpty(row["central email"])==false) && (String.IsNullOrWhiteSpace(comparePart.getValue("central email")) || String.IsNullOrEmpty(comparePart.getValue("central email"))))
            {
                return 1;
            }

            if ((String.IsNullOrWhiteSpace(row["central email"]) || String.IsNullOrEmpty(row["central email"])) && (String.IsNullOrWhiteSpace(comparePart.getValue("central email"))==false || String.IsNullOrEmpty(comparePart.getValue("central email")  )==false))
            {
                return -1;
            }


           
                return row["central email"].CompareTo(comparePart.getValue("central email"));
            

        }

        public bool Equals(Rowdata data)
        {
            if (data == null)
                return false;

            if (isNull(row["first name"]) == true && isNull(data.getValue("first name")) == true && isNull(row["last name"]) == true && isNull(data.getValue("last name")) == true)
            {
                //all variables are null, so don't even merge
                return false;
            }
            else if (isNull(row["first name"]) == false && isNull(data.getValue("first name")) == false && isNull(row["last name"]) == false && isNull(data.getValue("last name")) == false)
            {

                //none of the variables are null, so merge
                if (row["first name"].Equals(data.getValue("first name")) && row["last name"].Equals(data.getValue("last name")))
                {
                    return true;
                }
            }

            //every other case means one of them are null, so false
         

            return false;
        }
        //return true if item is null or empty
        private bool isNull(String item)
        {
            if (String.IsNullOrWhiteSpace(item) || String.IsNullOrEmpty(item))
            {
                return true;
            }
            return false;
        }

    }

    class Program
    {
        //all outputs goes in the order of title array
        public static List<String> titleArray = new List<String>();

        public static List<Rowdata> rowDataArray = new List<Rowdata>();

        //returns a filehandle in directory info if it exist, otherwise return null
        static DirectoryInfo fileHandle(String folderPath)
        {
            if (String.IsNullOrEmpty(folderPath) == true || String.IsNullOrWhiteSpace(folderPath) == true)
            {
                return null;
            }

            DirectoryInfo path = new DirectoryInfo(folderPath);

            if (path.Exists == true)
            {
                //directory exists for reading
                return path;
            }
            

            return null;

        }

        public static bool stringEmpty(String s)
        {
            if (String.IsNullOrWhiteSpace(s) == true || String.IsNullOrEmpty(s) == true)
            {
                return true;
            }

            return false;

        }

        //iterate through the spliced strings delimited by commas and added it title array
        static void csvIteratorTitleArray(String line)
        {

            if (stringEmpty(line))
            {
                return;
            }

          TextFieldParser parser = new TextFieldParser(line);
          parser.TextFieldType = FieldType.Delimited;
          parser.SetDelimiters(",");
          String[] fields = parser.ReadFields();
          if (fields != null)
          {
              for (int i = 0; i<fields.Length; i++)
              {
                  if (titleArray.Contains(fields[i].ToLower()) == false)
                  {
                      titleArray.Add(fields[i].ToLower());
                  }

                  
              }
          }
          parser.Close();
          parser = null;
        }

       //iterate through files and process the data rows
        

        //returns an arraylist of headers from all csv file
        public static ArrayList extractCSVHeader()
        {
            String path = "C:\\Users\\Randy\\Desktop\\CorrectedME";
            DirectoryInfo info = fileHandle(path);
            if (info == null)
                return null;

            //pre-initialize the title array
            titleArray = new List<string>();
            
            titleArray.Add("first name");
            titleArray.Add("last name");
            titleArray.Add("company");
            titleArray.Add("title");
            titleArray.Add("central email"); //do not add email that is already there, but add the ones that are different. [String]

            foreach (var fi in info.EnumerateFiles())
            {
                //read the first line of the text file and place them in header array
                csvIteratorTitleArray(fi.FullName);
                

            }

            Console.WriteLine("Printing Title Array: ");
            titleArray.ForEach(delegate(String item)
            {
                Console.WriteLine(item);
            });
            Console.WriteLine("PRINTING ENDS-------------------------------");

            //going through each files and make modifications to titleArray
            foreach (var fi in info.EnumerateFiles())
            {
            
                TextFieldParser parser = new TextFieldParser(fi.FullName);
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

              
                if (!parser.EndOfData)
                {

                    String[] fields = parser.ReadFields(); //first line then advanced
                 /*   List<String> curHeader = new List<String>();
                    //iterating through all fields on the first row
                    for (int i = 0; i < fields.Length; i++)
                    {
                        curHeader.Add(fields[i].ToLower());
                    }

                    curHeader.ForEach(delegate(String data)
                    {
                        Console.WriteLine("Thing::: "+ data);
                    });*/


                    //going through the remainder lines
                    Rowdata curRow;
                    while (!parser.EndOfData)
                    {
                        String[] moreFields = parser.ReadFields();
                        curRow = new Rowdata();
                        for (int i = 0; i < moreFields.Length; i++)
                        {
                           
                            curRow.setValue(fields[i].ToLower(), moreFields[i].ToLower());


                        }
                        rowDataArray.Add(curRow);
                       

                    }

                }
                else
                {
                    //current first line header is nothing, hence we do nothing
                }

                //read the remainder lines
                parser.Close();
                parser = null;

            }
            


            return null;
        }

 
        public static void outputRowDataViaInternal()
        {
       
            for (int i = 0; i < rowDataArray.Count; i++)
            {
                rowDataArray[i].output();

            }
        }
        //null if line is empty, otherwise this will return a list of string
        public static List<String> extractCurrentHeader(String line)
        {
            if (stringEmpty(line))
            {
                return null;
            }

            List<String> newList = new List<String>();

            String[] split = line.Split(new Char[] { ',' });
            

            foreach (var sb in split)
            {
              

                String currentItem = sb.Trim(new Char[] { '"',' ' });
                //insert the item into titlearray if it doesn't already exist in it
                
                if (newList.Contains(currentItem.ToLower()) == false)
                {
                    newList.Add(currentItem.ToLower());
                }
                
            }

            return newList;

        }

        public static List<String> extractDataRow(String line)
        {
            if (stringEmpty(line))
            {
                return null;
            }
            List<String> newList = new List<String>();
            String currentItem;

            String[] split = line.Split(new Char[] { ',' });
            foreach (var sb in split)
            {


                currentItem = sb.Trim(new Char[] { '"', ' ' });
                //insert the item into titlearray if it doesn't already exist in it
                //Console.WriteLine(currentItem.ToLower() + "###");

           
                newList.Add(currentItem.ToLower());
                

            }


            return newList;

        }

        static void writeRowDataToFile()
        {
            String path = "C:\\Users\\Randy\\Desktop\\output\\output.csv";
            if (File.Exists(path) == false)
            {
                File.Create(path).Close();
                Console.WriteLine("Output File Created");

            }
            else
            {

                File.Delete(path);
                File.Create(path).Close();
                Console.WriteLine("Output File Replaced by a New File");
            }

            StreamWriter newWriter = new StreamWriter(path, true);

            //write headers
            newWriter.Write("central email" + ",");
            newWriter.Flush();
            newWriter.Write("last name" + ",");
            newWriter.Flush();

            newWriter.Write("first name" + ",");
            newWriter.Flush();

            newWriter.Write("full name" + ",");
            newWriter.Flush();


            newWriter.Write("company" + ",");
            newWriter.Flush();

            newWriter.Write("title" + ",");
            newWriter.Flush();

            newWriter.WriteLine();
            newWriter.Flush();

        

                //write the data fields
                for (int i = 0; i < rowDataArray.Count; i++)
                {
                    //row that needs to be outputted
                    Rowdata curData = rowDataArray[i];

                    curData.writeToStream(newWriter);

                }
                newWriter.Flush();
                newWriter.Close();
        }



        static bool removingBlanks()
        {
           

            if (rowDataArray == null)
                return false;
            Console.WriteLine("Total Line Count: " + rowDataArray.Count);

            List<Rowdata> newRowData = new List<Rowdata>();

            int count = 0;
            rowDataArray.ForEach(delegate(Rowdata data)
            {

                //Console.WriteLine(count);
                //Console.WriteLine("First Name: " + data.getValue("first name") + " Last Name: " + data.getValue("last name") + " Middle Name: " + data.getValue("middle name") + " Full Name: " + data.getValue("full name"));


                if (String.IsNullOrWhiteSpace(data.getValue("first name")) == true && String.IsNullOrWhiteSpace(data.getValue("last name")) == true )
                {
                    //removeable
                }
                else
                {
                    //addable
                    newRowData.Add(data);
                }
                
               
                
                count++;
            }

                );

           //done, now simply replace the rowDataArray with new rowDataArray
            rowDataArray = newRowData;
            

            outputRowdataArray(); //output the row data

                return true;
        }

        static void outputRowdataArray(){


            int count = 0;
            rowDataArray.ForEach(delegate(Rowdata data)
            {

                //Console.WriteLine(count);
                //Console.WriteLine("First Name: " + data.getValue("first name") + " Last Name: " + data.getValue("last name") + " Middle Name: " + data.getValue("middle name") + " Full Name: " + data.getValue("full name"));

                count++;
            }

                );
        }
        static Rowdata dataMerge(Rowdata data1, Rowdata data2)
        {
            if (data1 == null && data2 == null)
                return null;

            Rowdata newRowData = new Rowdata(data1);
            List<String> customArray = new List<String>();
            customArray.Add("central email");
            customArray.Add("last name");
            customArray.Add("first name");
            customArray.Add("full name");
            customArray.Add("company");
            customArray.Add("title");

            for (int i = 0; i < customArray.Count; i++)
            {
                if (customArray[i].Equals("first name") || customArray[i].Equals("last name") )
                {
                    continue;
                }


                if (String.IsNullOrEmpty(newRowData.getValue(customArray[i])) || String.IsNullOrWhiteSpace(newRowData.getValue(customArray[i])))
                {
                    newRowData.setValue(customArray[i], data2.getValue(customArray[i]));

                }
                else
                {
                   
                    if (String.IsNullOrEmpty(data2.getValue(customArray[i])) || String.IsNullOrWhiteSpace(data2.getValue(customArray[i])))
                    {
                        //data2 has empty field we keep newrowdata and do nothing to it

                    }
                    else
                    {
                        //data2 doesn't have empty field we concat
                        newRowData.setValue(customArray[i], String.Concat(newRowData.getValue(customArray[i]), "   "+data2.getValue(customArray[i])));

                    }    
                }


            }

            return newRowData;

        }

        //merge duplicate entries by merging them
        static void mergeSimilarFLM()
        {

            List<Rowdata> saveList = new List<Rowdata>();
            Rowdata curRowData = rowDataArray[0]; //initialize it to the first element

            for (int i = 1; i < rowDataArray.Count; i++)
            {
                //Console.WriteLine("First Name: " + rowDataArray[i].getValue("first name") + " Last Name: " + rowDataArray[i].getValue("last name") + " Middle Name: " + rowDataArray[i].getValue("middle name") + " Full Name: " + rowDataArray[i].getValue("full name"));
                if (curRowData.Equals(rowDataArray[i]) == false)
                {
                    //no
                    saveList.Add(curRowData); //saving the item from the curRowData
                    curRowData = rowDataArray[i]; //get the current iterating row as row data

                }
                else
                {
                    //yes, we now merge curRowData with the rowData being iterated.
                    Rowdata newData = dataMerge(curRowData, rowDataArray[i]);
                    if (newData != null)
                    {
                        curRowData = newData;
                    }
                    

                }

            }

            //process the last row
            saveList.Add(rowDataArray[rowDataArray.Count-1]);

            rowDataArray = saveList;

        }

        public static void fullNameSplit()
        {
            String curval;
            String fval;
            String lval;
            //go through the entire array line by line
            for (int i = 0; i < rowDataArray.Count; i++)
            {
                curval = rowDataArray[i].getValue("full name");
                if (String.IsNullOrEmpty(curval) || String.IsNullOrWhiteSpace(curval))
                {
                    //if the field is empty or null, then simply skip the current
                    continue;
                }



                if (String.IsNullOrEmpty(rowDataArray[i].getValue("first name")) || String.IsNullOrWhiteSpace(rowDataArray[i].getValue("first name")))
                {
                    //if the field is empty or null, then simply skip the current
                    try
                    {
                        fval = curval.Substring(0, curval.IndexOf(' '));
                        //Console.WriteLine("first name from split: " + fval);
                        rowDataArray[i].setValue("first name", fval);
                    }
                    catch (Exception e)
                    {
                        //do nothing because there is no divider
                    }
                   
                }

                if ((String.IsNullOrEmpty(rowDataArray[i].getValue("last name")) || String.IsNullOrWhiteSpace(rowDataArray[i].getValue("last name"))))
                {
                    try
                    {
                        //if the field is empty or null, then simply skip the current
                        lval = curval.Substring(curval.IndexOf(' '));
                        //Console.WriteLine("last name from split: " + lval);
                        rowDataArray[i].setValue("last name", lval);
                    }
                    catch (Exception e)
                    {
                        //do nothing because there is no space divider
                    }

                    
                }


            }

        }
        public static void fullNameRestore()
        {
            String curval;
            String fval;
            String lval;

            //go through the entire array line by line
            for (int i = 0; i < rowDataArray.Count; i++)
            {
                curval = rowDataArray[i].getValue("full name");
                if (String.IsNullOrEmpty(curval) || String.IsNullOrWhiteSpace(curval))
                {
                    //do something to it
                    if (String.IsNullOrEmpty(rowDataArray[i].getValue("first name")) == false || String.IsNullOrWhiteSpace(rowDataArray[i].getValue("first name")) == false)
                    {
                        //if the field is empty or null, then simply skip the current

                        fval = rowDataArray[i].getValue("first name");
                        Console.WriteLine("first name from split: " + rowDataArray[i].getValue("first name"));
                        rowDataArray[i].setValue("full name", fval);
                    }

                    if (String.IsNullOrEmpty(rowDataArray[i].getValue("last name")) == false || String.IsNullOrWhiteSpace(rowDataArray[i].getValue("last name")) == false)
                    {
                        lval = rowDataArray[i].getValue("last name");
                        Console.WriteLine("last name from split: " + rowDataArray[i].getValue("last name"));


                        //already filled by first name
                        if (String.IsNullOrEmpty(curval) == false || String.IsNullOrWhiteSpace(curval) == false)
                        {
                            
                            rowDataArray[i].setValue("full name", String.Concat(rowDataArray[i].getValue("full name"),  " " +lval));
                        }

                        //rowDataArray[i].setValue("full name", lval);
                        rowDataArray[i].setValue("full name", String.Concat(rowDataArray[i].getValue("full name"), " " + lval));
                    }


                }
                //do nothing since it isn't empty or null


            }
        }

        public static void emailRestore()
        {

            //test starts here
            //Console.WriteLine(isEmail("asdfawe o'reilly@there.com eagwea21"));

            String curVal;
            String curEmail;
            //actual code starts here

            //go through each value in therow dataarray
            for (int i = 0; i < rowDataArray.Count; i++)
            {
                
                //go through each fields in the row
                for (int j = 0; j < titleArray.Count; j++)
                {
                    if (String.Equals(titleArray[j], "central email"))
                    {
                        //if central email is encountered, we simply skip the current iteration because we are messing with central email
                        continue;
                    }

                    curVal = rowDataArray[i].getValue(titleArray[j]);
                    curEmail = isEmail(curVal);
                    if ( curEmail != null)
                    {
                        //curval contains an email so we must put it in current rowDataArray's central email, but before that we have to make sure it doesnt already have it
                        
                        //already have it
                        if(centralContain(rowDataArray[i].getValue("central email"), curEmail)){

                            //do nothing, the central email already contain the curemail

                        }
                        else
                        {
                            //doesn't have it yet, we do some concat
                            rowDataArray[i].setValue("central email", String.Concat(rowDataArray[i].getValue("central email"), " " + curEmail));

                        }


                    }

                }

                    

            }



        }
        //returns true if data2 is already inside centralEmailString, otherwise it will return false
        public static bool centralContain(String centralEmailString, String data2)
        {
            if (String.IsNullOrEmpty(centralEmailString) || String.IsNullOrWhiteSpace(centralEmailString))
            {
                return false;
            }

            if (String.IsNullOrEmpty(data2) || String.IsNullOrWhiteSpace(data2))
            {
                return false;
            }

            if (centralEmailString.IndexOf(data2) != -1)
            {
                return true;
            }
            return false;
        }

        //returns an email from the string if there is one, otherwise return null
        public static String isEmail(String data)
        {
            if (String.IsNullOrEmpty(data) || String.IsNullOrWhiteSpace(data))
            {
                return null;
            }

            string emailPattern = @"[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*@((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))";

            Regex remailPattern = new Regex(emailPattern);

            MatchCollection newCollection;

            newCollection = remailPattern.Matches(data);

            for (int i = 0; i < newCollection.Count; i++)
            {
                Console.WriteLine("{0}. {1}", i, newCollection[i].Value);
                return newCollection[i].Value;
            }
           


                return null;
        }

        //keep only one entry with the same email
        static void distinctByEmail()
        {
            List<Rowdata> retList = new List<Rowdata>();
            Rowdata curRow;
            //go through each items in rowdata only add email items that retlist doesn't have
            for (int i = 0; i < rowDataArray.Count; i++)
            {
                curRow = rowDataArray[i];

                if (retList.Exists(x => x.EqualsEmail(curRow.getValue("central email"))))
                {
                    continue;
                }
                else
                {
                    retList.Add(curRow);
                }

            }

           
            rowDataArray = retList;
            
        }

        //if the first and the last name are the same, then do a centralcontain on centralemail and if 
        //not exist concat, otherwise do nothing
        static void distinctByFLName()
        {
            List<Rowdata> retList = new List<Rowdata>();
            Rowdata curRow;
            Rowdata modRow;
            for (int i = 0; i < rowDataArray.Count; i++)
            {
                curRow= rowDataArray[i];
                modRow = flcontain(retList, curRow);
                if (modRow != null)
                {
                    if (centralContain(curRow.getValue("central email"), modRow.getValue("central email") ))
                    {
                        //does contain, do nothing


                    }
                    else
                    {
                        //does not contain, do concat
                        modRow.setValue("central email", String.Concat(curRow.getValue("central email"), " " + modRow.getValue("central email")));

                    }
                }
                else
                {
                    retList.Add(curRow);
                }

                //retlist doesn't contain this item
                

            }

            
            rowDataArray = retList;

        }

        //find the element that has the same first and last name, if exist, return it, if not return null
        static Rowdata flcontain(List<Rowdata> retList, Rowdata curRow )
        {
            //go through each element in retlist to find a match, if a match is found break out immediately
            for (int j = 0; j < retList.Count; j++)
            {
                //first and last name match found
                if (retList[j].Equals(curRow))
                {

                    return retList[j];
             

                }
                else
                {
                    //do nothing, and keep going until you find a match
                }

            }

            return null;
        }

        //place of execution
        static void Main(string[] args)
        {


            //extract the rowDatas onto the data structure
            extractCSVHeader();

            //remove all the rowdatas without first name, last name, middle name and full name
            /*if (removingBlanks() == false)
            {
                Console.WriteLine("Removing Blanks Failed");
            }else{
                Console.WriteLine("Removing Blanks Successful");
            }*/

            //split full name into first name and last name
            fullNameSplit();

            //going through each item in the title array and use it to find if there is email in the dictionary and execute merge if email doesn't have an entry
            fullNameRestore();

            emailRestore();



            //sorting the list in order of last name
            Console.WriteLine("Sorting Array");
            rowDataArray.Sort();
            
            //eliminate duplicates by merging everything other than first name, last name, middle name, we use first name, last name, and middle name as equality determinant
            //mergeSimilarFLM();

            distinctByEmail();
            //outputRowdataArray();
            //distinctByFLName();



           writeRowDataToFile(); //write when data processed is correct
           titleArray = null; //clear out the garbages
           rowDataArray = null;
           GC.Collect();
            
            
            Console.ReadKey();

        }
    }

}
