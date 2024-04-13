/* Title:           MDU Letters Class
 * Date:            10-9-17
 * Author:          Terry Holmes
 * 
 * Description:     This the class for MDU Letters */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewEventLogDLL;
using LettersDLL;
using DropBuryMDUDLL;
using CustomersDLL;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MDUDropBury
{
    class MDULettersClass
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        LettersClass TheLettersClass = new LettersClass();
        DropBuryMDUClass TheDropBuryMDUClass = new DropBuryMDUClass();
        CustomersClass TheCustomersClass = new CustomersClass();

        //setting up the data
        FindLetterParagraphByLetterIDDataSet TheFindLetterParagraphByLetterIDDataSet = new FindLetterParagraphByLetterIDDataSet();
        FindCustomerByAccountNumberDataSet TheFindCustomerByAccountNumberDataSet = new FindCustomerByAccountNumberDataSet();
        FindLetterByDescriptionDataSet TheFindLetterByDescriptionDataSet = new FindLetterByDescriptionDataSet();

        public void CreateMDUDropAcceptanceLetter(string strAccountNumber, string strCallingProgram)
        {
            //this will print the report
            
            string strFirstName;
            string strLastName;
            string strPhoneNumber;
            string strAddress;
            int intCounter;
            int intNumberOfRecords;
            int intLetterID = 0;
            
            try
            {
                PrintDialog pdAcceptLetter = new PrintDialog();

                TheFindLetterByDescriptionDataSet = TheLettersClass.FindLetterByDescription("MDU DROP ACCEPTANCE AND RELEASE");

                if (pdAcceptLetter.ShowDialog().Value)
                {
                    FlowDocument fdAcceptLetter = new FlowDocument();
                    Paragraph Title = new Paragraph(new Run("BLUE JAY COMMUNICATIONS, INC"));
                    Title.FontSize = 20;
                    Title.TextAlignment = TextAlignment.Center;
                    Title.LineHeight = 1;
                    fdAcceptLetter.Blocks.Add(Title);
                    Paragraph Title2 = new Paragraph(new Run("7500 Associates Avenue"));
                    Title2.FontSize = 16;
                    Title2.LineHeight = 1;
                    Title2.TextAlignment = TextAlignment.Center;
                    fdAcceptLetter.Blocks.Add(Title2);
                    Paragraph Title3 = new Paragraph(new Run("Brooklyn, OH 44144"));
                    Title3.FontSize = 16;
                    Title3.LineHeight = 1;
                    Title3.TextAlignment = TextAlignment.Center;
                    fdAcceptLetter.Blocks.Add(Title3);
                    fdAcceptLetter.ColumnWidth = 900;
                    Paragraph Space1 = new Paragraph(new Run());
                    Space1.LineHeight = 1;
                    fdAcceptLetter.Blocks.Add(Space1);
                    Paragraph Title4 = new Paragraph(new Run("ACCEPTANCE OF COMPLETED WORK AND"));
                    Title4.TextDecorations = TextDecorations.Underline;
                    Title4.FontSize = 16;
                    Title4.LineHeight = 1;
                    Title4.TextAlignment = TextAlignment.Center;
                    fdAcceptLetter.Blocks.Add(Title4);
                    Paragraph Title5 = new Paragraph(new Run("WAIVER OF LIABILITY"));
                    Title5.TextDecorations = TextDecorations.Underline;
                    Title5.FontSize = 16;
                    Title5.LineHeight = 1;
                    Title5.TextAlignment = TextAlignment.Center;
                    fdAcceptLetter.Blocks.Add(Title5);
                    Paragraph Space2 = new Paragraph(new Run());
                    Space1.LineHeight = 2;
                    fdAcceptLetter.Blocks.Add(Space2);
                    
                    //getting the customer information
                    TheFindCustomerByAccountNumberDataSet = TheCustomersClass.FindCustomerByAccountNumber(strAccountNumber);

                    strFirstName = TheFindCustomerByAccountNumberDataSet.FindCustomerByAccountNumber[0].FirstName;
                    strLastName = TheFindCustomerByAccountNumberDataSet.FindCustomerByAccountNumber[0].LastName;
                    strPhoneNumber = TheFindCustomerByAccountNumberDataSet.FindCustomerByAccountNumber[0].PhoneNumber;
                    strAddress = TheFindCustomerByAccountNumberDataSet.FindCustomerByAccountNumber[0].StreetAddress;

                    Paragraph CustomerLine = new Paragraph(new Run("Customer(s) Name: \t" + strFirstName + " " + strLastName));
                    CustomerLine.FontSize = 16;
                    CustomerLine.LineHeight = 3;
                    CustomerLine.TextAlignment = TextAlignment.Left;
                    fdAcceptLetter.Blocks.Add(CustomerLine);

                    Paragraph AddressLine = new Paragraph(new Run("Address: \t\t" + strAddress));
                    AddressLine.FontSize = 16;
                    AddressLine.LineHeight = 3;
                    AddressLine.TextAlignment = TextAlignment.Left;
                    fdAcceptLetter.Blocks.Add(AddressLine);

                    Paragraph PhoneLine = new Paragraph(new Run("Phone Number: \t\t" + strPhoneNumber));
                    PhoneLine.FontSize = 16;
                    PhoneLine.LineHeight = 3;
                    PhoneLine.TextAlignment = TextAlignment.Left;
                    fdAcceptLetter.Blocks.Add(PhoneLine);

                    Paragraph Space3 = new Paragraph(new Run());
                    Space3.LineHeight = 2;
                    fdAcceptLetter.Blocks.Add(Space3);

                    intLetterID = TheFindLetterByDescriptionDataSet.FindLetterByDescription[0].LetterID;

                    TheFindLetterParagraphByLetterIDDataSet = TheLettersClass.FindLetterParagraphByLetterID(intLetterID);

                    intNumberOfRecords = TheFindLetterParagraphByLetterIDDataSet.FindLetterParagraphByLetterID.Rows.Count - 1;

                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        Paragraph ParagraphText = new Paragraph(new Run("\t" + TheFindLetterParagraphByLetterIDDataSet.FindLetterParagraphByLetterID[intCounter].ParagraphText));
                        ParagraphText.FontSize = 16;
                        ParagraphText.LineHeight = 3;
                        ParagraphText.TextAlignment = TextAlignment.Left;
                        fdAcceptLetter.Blocks.Add(ParagraphText);

                        Paragraph Space4 = new Paragraph(new Run());
                        Space4.LineHeight = 2;
                        fdAcceptLetter.Blocks.Add(Space4);
                    }

                    Paragraph Comments1 = new Paragraph(new Run("Comments: ____________________________________________________________"));
                    Comments1.FontSize = 16;
                    Comments1.LineHeight = 10;
                    Comments1.TextAlignment = TextAlignment.Left;
                    fdAcceptLetter.Blocks.Add(Comments1);

                    Paragraph Comments2 = new Paragraph(new Run("____________________________________________________________________"));
                    Comments2.FontSize = 16;
                    Comments2.LineHeight = 10;
                    Comments2.TextAlignment = TextAlignment.Left;
                    fdAcceptLetter.Blocks.Add(Comments2);

                    Paragraph Comments3 = new Paragraph(new Run("____________________________________________________________________"));
                    Comments3.FontSize = 16;
                    Comments3.LineHeight = 10;
                    Comments3.TextAlignment = TextAlignment.Left;
                    fdAcceptLetter.Blocks.Add(Comments3);

                    Paragraph Space5 = new Paragraph(new Run());
                    Space5.LineHeight = 10;
                    fdAcceptLetter.Blocks.Add(Space5);

                    Paragraph FinalLine1 = new Paragraph(new Run("Date: _________\t\t\t\t\t" + "__________________________"));
                    FinalLine1.FontSize = 16;
                    FinalLine1.LineHeight = 1;
                    FinalLine1.TextAlignment = TextAlignment.Left;
                    fdAcceptLetter.Blocks.Add(FinalLine1);

                    Paragraph FinalLine2 = new Paragraph(new Run("\t\t\t\t\t\t\tCustomer Signature"));
                    FinalLine2.FontSize = 16;
                    FinalLine2.LineHeight = 1;
                    FinalLine2.TextAlignment = TextAlignment.Left;
                    fdAcceptLetter.Blocks.Add(FinalLine2);

                    Paragraph Space6 = new Paragraph(new Run());
                    Space6.LineHeight = 6;
                    fdAcceptLetter.Blocks.Add(Space6);

                    Paragraph FinalLine3 = new Paragraph(new Run("\t\t\t\t\t\t\t__________________________"));
                    FinalLine3.FontSize = 16;
                    FinalLine3.LineHeight = 1;
                    FinalLine3.TextAlignment = TextAlignment.Left;
                    fdAcceptLetter.Blocks.Add(FinalLine3);

                    Paragraph FinalLine4 = new Paragraph(new Run("\t\t\t\t\t\t\tCustomer Signature"));
                    FinalLine4.FontSize = 16;
                    FinalLine4.LineHeight = 1;
                    FinalLine4.TextAlignment = TextAlignment.Left;
                    fdAcceptLetter.Blocks.Add(FinalLine4);

                    Paragraph Space7 = new Paragraph(new Run());
                    Space6.LineHeight = 3;
                    fdAcceptLetter.Blocks.Add(Space7);

                    Paragraph FinalLine5 = new Paragraph(new Run("\t\t\t\t\t\t\t__________________________"));
                    FinalLine5.FontSize = 16;
                    FinalLine5.LineHeight = 1;
                    FinalLine5.TextAlignment = TextAlignment.Left;
                    fdAcceptLetter.Blocks.Add(FinalLine5);

                    Paragraph FinalLine6 = new Paragraph(new Run("\t\t\t\t\t\t\tTechnician's Signature"));
                    FinalLine6.FontSize = 16;
                    FinalLine6.LineHeight = 1;
                    FinalLine6.TextAlignment = TextAlignment.Left;
                    fdAcceptLetter.Blocks.Add(FinalLine6);
                    
                    Thickness thickness = new Thickness(50, 50, 50, 50);
                    fdAcceptLetter.PagePadding = thickness;
                                   

                    pdAcceptLetter.PrintDocument(((IDocumentPaginatorSource)fdAcceptLetter).DocumentPaginator, "Blue Jay Communications Acceptance");
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, strCallingProgram + "MDU Letters Class // Create MDU Drop Acceptance Letter " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
