using Aspose.Cells;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using MTBrokerMobile.ControllerReturnTypes.MessageMngt;
using MTBrokerMobile.DependencyServiceInterfaces;
using MTBrokerMobile.StaticHelpers;
using MTBrokerMobile.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Cell = DocumentFormat.OpenXml.Spreadsheet.Cell;

namespace MTBrokerMobile.ViewModels
{
    internal class ParsedMT940ViewModel : INotifyPropertyChanged
    {
        #region Properties

        public Command CancelCommand { get; private set; }


        public Command DownloadPDFCommand => new Command(execute: async () =>
        {

            try
            {
                IsBusy = true;


                var fileName = await SaveExcelAsync();

                var pdfFileName = $"{Path.GetFileNameWithoutExtension(fileName)}.pdf";

                   // load the XLSX file in an instance of Workbook
                   var book = new Aspose.Cells.Workbook(fileName);
                // save XLSX as PDF
                book.Save($"{pdfFileName}", SaveFormat.Auto);

                File.Delete(fileName);

                await Launcher.OpenAsync(pdfFileName);

                IsBusy = false;
            }
            catch(Exception exc)
            {

            }

        });


        public Command DownloadXLSXCommand => new Command(execute: async () =>
        {
            await SaveExcelAsync();
        });


        public Command RefreshCommand { get; private set; }



        public Command OrderByDateAscCommand => new Command(execute: async () =>
        {
            MT940CRTs.OrderBy(n => n.MT940MonoCRT.AvailableBalance64Date);
        });

        public Command OrderByDateDescCommand => new Command(execute: async () =>
        {
            MT940CRTs.OrderByDescending(n => n.MT940MonoCRT.AvailableBalance64Date);
        });


        public Command ToggleBlock1Command => new Command(execute: async () =>
        {
            ShowBlock1 = !ShowBlock1;
            OnPropertyChanged(nameof(ShowBlock1));
        });


        private bool isBusy = false;
        public bool IsBusy
        {
            get => isBusy;
            set => SetProperty(ref isBusy, value);
        }

        private bool showBlock1 = false;
        public bool ShowBlock1
        {
            get => showBlock1;
            set => SetProperty(ref showBlock1, value);
        }


        private bool userIsLoggedIn = false;
        public bool UserIsLoggedIn
        {
            get => userIsLoggedIn;
            set { SetProperty(ref userIsLoggedIn, value); }
        }

        ObservableCollection<MT940CRT> mT940CRTs = new ObservableCollection<MT940CRT>();

        public ObservableCollection<MT940CRT> MT940CRTs
        {
            get => mT940CRTs;
            set => SetProperty(ref mT940CRTs, value);
        }

        //flag disambiguating between Flyout Launch and Launch after file was parsed
        bool isInAutoMode = false;
        public bool IsInAutoMode
        {
            get => isInAutoMode;
            set => SetProperty(ref isInAutoMode, value);
        }

        public ParsedMT940Page EntityPage { get; set; }

        #endregion


        #region Constructors

        public ParsedMT940ViewModel()
        {
            MessagingCenter.Subscribe<object>(this, StaticVariables.ShareMT940Message, (inputMT940s) =>
            {
                if ((ParseMT940WithStatusCRT)inputMT940s != null && (bool)((ParseMT940WithStatusCRT)inputMT940s).SuccessStatusMessageCRT?.Success && ((ParseMT940WithStatusCRT)inputMT940s).MT940CRTs.Count > 0) (inputMT940s as ParseMT940WithStatusCRT).MT940CRTs.ForEach(n => MT940CRTs.Add(n));
            });


            MessagingCenter.Subscribe<object>(this, StaticVariables.ShareMultipleMT940Message, (inputMT940s) =>
            {
                if ((List<MT940CRT>)inputMT940s != null && ((List<MT940CRT>)inputMT940s).Count > 0) (inputMT940s as List<MT940CRT>).ForEach(n => MT940CRTs.Add(n));
            });


            CancelCommand = new Command(execute: () =>
            {
                IsBusy = false;
                EntityPage.Navigation.PopModalAsync();
            });


            RefreshCommand = new Command(execute: async () => await PopulateListAsync());
        }

        public ParsedMT940ViewModel(ParsedMT940Page parsedMT940Page) : this()
        {
            EntityPage = parsedMT940Page;

            IsInAutoMode = true;

            RefreshCommand.Execute(null);
        }


        public ParsedMT940ViewModel(ParsedMT940Page parsedMT940Page, bool isInManualMode) : this()
        {
            EntityPage = parsedMT940Page;
        }

        #endregion


        #region Construct Cell
        /* To create cell in Excel */
        private Cell ConstructCell(string value, CellValues dataType)
        {
            return new Cell()
            {
                CellValue = new CellValue(value),
                DataType = new EnumValue<CellValues>(dataType)
            };
        }
        #endregion


        #region Save Excel
        private async Task<string> SaveExcelAsync()
        {
            var fileNm = string.Empty;
            IsBusy = true;

            try
            {
                if (MT940CRTs.Count == 0)
                {
                    await EntityPage.DisplayAlert(StaticVariables.AppName, StaticVariables.NoDataToSaveErrorMessage, StaticVariables.Cancel);

                    IsBusy = false;

                    return fileNm;
                }

                var externalStoragePath = DependencyService.Get<IExternalStorageService>().GetExternalStoragePath();

                if (string.IsNullOrEmpty(externalStoragePath)) externalStoragePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                var pathIsWritable = DependencyService.Get<IExternalStorageService>().IsExternalStoragePathWritable();

                if (pathIsWritable)
                {
                    string date = " " + DateTime.Now.ToShortDateString() + "_" + DateTime.Now.ToLongTimeString();
                    date = date.Replace("/", "_");
                    date = date.Replace(":", "_");

                    var folderPath = Path.Combine(new string[] { externalStoragePath, StaticVariables.DownloadsFolder });

                    if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                    var fileName = Path.Combine(folderPath, $"{StaticVariables.AppName}{date}.xlsx");

                    using (SpreadsheetDocument document = SpreadsheetDocument.Create(fileName, SpreadsheetDocumentType.Workbook))
                    {
                        WorkbookPart workbookPart = document.AddWorkbookPart();
                        workbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();

                        WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                        worksheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet();

                        Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                        Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "MT940" };
                        sheets.Append(sheet);

                        workbookPart.Workbook.Save();

                        SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                        // Constructing header
                        var row = new DocumentFormat.OpenXml.Spreadsheet.Row();

                        row.Append(
                            ConstructCell(StaticVariables.FinBranchCode, CellValues.String),
                            ConstructCell(StaticVariables.FinLTCode, CellValues.String),
                            ConstructCell(StaticVariables.FinSwiftAddress, CellValues.String),
                            ConstructCell(StaticVariables.SendersSwiftAddress, CellValues.String),
                            ConstructCell(StaticVariables.Block1SequenceNumber, CellValues.String),
                            ConstructCell(StaticVariables.Block1SessionNumber, CellValues.String),
                            ConstructCell(StaticVariables.AccountID25, CellValues.String),
                            ConstructCell(StaticVariables.DebitOrCredit, CellValues.String),
                            ConstructCell(StaticVariables.AvailableBalance64Amount, CellValues.String),
                            ConstructCell(StaticVariables.Currency, CellValues.String),
                            ConstructCell(StaticVariables.Date, CellValues.String),
                             ConstructCell(StaticVariables.DebitOrCredit, CellValues.String),
                            ConstructCell(StaticVariables.OpeningBalance60FAmount, CellValues.String),
                            ConstructCell(StaticVariables.Currency, CellValues.String),
                            ConstructCell(StaticVariables.Date, CellValues.String),
                            ConstructCell(StaticVariables.DebitOrCredit, CellValues.String),
                            ConstructCell(StaticVariables.ClosingBalance62FAmount, CellValues.String),
                            ConstructCell(StaticVariables.Currency, CellValues.String),
                            ConstructCell(StaticVariables.Date, CellValues.String),
                            ConstructCell(StaticVariables.StatementOrSeqNo28CMsgSeq, CellValues.String),
                            ConstructCell(StaticVariables.StatementOrSeqNo28CStmntSeq, CellValues.String),

                                  ConstructCell(StaticVariables.AccOwnerInfo86Info, CellValues.String),
                                  ConstructCell(StaticVariables.DebitOrCredit, CellValues.String),
                                  ConstructCell(StaticVariables.StatementLine61Amount, CellValues.String),
                                  ConstructCell(StaticVariables.Date, CellValues.String),
                                  ConstructCell(StaticVariables.StatementLine61CustomerRef, CellValues.String),
                                  ConstructCell(StaticVariables.StatementLine61TrnsactnTypeID, CellValues.String),
                                  ConstructCell(StaticVariables.StatementLine61ValueDate, CellValues.String)
                                  );

                        // Insert the header row to the Sheet Data
                        sheetData.AppendChild(row);

                        // Add each product
                        foreach (var d in MT940CRTs)
                        {
                            row = new DocumentFormat.OpenXml.Spreadsheet.Row();
                            row.Append(
                                ConstructCell(d.MT940MonoCRT.FinBranchCode, CellValues.String),
                                ConstructCell(d.MT940MonoCRT.FinLTCode, CellValues.String),
                                ConstructCell(d.MT940MonoCRT.FinSwiftAddress, CellValues.String),
                                ConstructCell(d.MT940MonoCRT.SendersSwiftAddress, CellValues.String),
                                ConstructCell(d.MT940MonoCRT.SequenceNumber, CellValues.String),
                                ConstructCell(d.MT940MonoCRT.SessionNumber, CellValues.String),
                                ConstructCell(d.MT940MonoCRT.AccountID25, CellValues.String),
                                ConstructCell(d.MT940MonoCRT.AvailableBalance64DOrC, CellValues.String),
                                ConstructCell(d.MT940MonoCRT.AvailableBalance64Amount.ToString(), CellValues.String),
                                ConstructCell(d.MT940MonoCRT.AvailableBalance64Currency, CellValues.String),
                                ConstructCell(d.MT940MonoCRT.AvailableBalance64Date.ToShortDateString(), CellValues.String),
                                ConstructCell(d.MT940MonoCRT.OpeningBalance60FDOrC, CellValues.String),
                                ConstructCell(d.MT940MonoCRT.OpeningBalance60FAmount.ToString(), CellValues.String),
                                ConstructCell(d.MT940MonoCRT.OpeningBalance60FCurrency, CellValues.String),
                                ConstructCell(d.MT940MonoCRT.OpeningBalance60FDate.ToShortDateString(), CellValues.String),
                                 ConstructCell(d.MT940MonoCRT.ClosingBalance62FDOrC, CellValues.String),
                                ConstructCell(d.MT940MonoCRT.ClosingBalance62FAmount.ToString(), CellValues.String),
                                ConstructCell(d.MT940MonoCRT.ClosingBalance62FCurrency, CellValues.String),
                                ConstructCell(d.MT940MonoCRT.ClosingBalance62FDate.ToShortDateString(), CellValues.String),
                                ConstructCell(d.MT940MonoCRT.StatementOrSeqNo28CMsgSeq, CellValues.String),
                                ConstructCell(d.MT940MonoCRT.StatementOrSeqNo28CStmntSeq, CellValues.String),


                                //
                                ConstructCell(d.Tag61And86GroupCRTs[0]?.AccOwnerInfo86Info ?? string.Empty, CellValues.String),
                                 ConstructCell(d.Tag61And86GroupCRTs[0]?.StatementLine61DOrC ?? string.Empty, CellValues.String),
                                ConstructCell(d.Tag61And86GroupCRTs[0]?.StatementLine61Amount.ToString(), CellValues.String),
                                ConstructCell(d.Tag61And86GroupCRTs[0]?.StatementLine61EntryDate, CellValues.String),
                                ConstructCell(d.Tag61And86GroupCRTs[0].StatementLine61CustomerRef ?? string.Empty, CellValues.String),
                                ConstructCell(d.Tag61And86GroupCRTs[0]?.StatementLine61TrnsactnTypeID ?? string.Empty, CellValues.String),
                                ConstructCell(d.Tag61And86GroupCRTs[0]?.StatementLine61ValueDate.ToShortDateString(), CellValues.String)
                                );


                            sheetData.AppendChild(row);

                            for (int contIndex = 1; contIndex < d.Tag61And86GroupCRTs.Count; contIndex++)
                            {
                                row = new DocumentFormat.OpenXml.Spreadsheet.Row();

                                row.Append(
                                ConstructCell(string.Empty, CellValues.String),
                                ConstructCell(string.Empty, CellValues.String),
                                ConstructCell(string.Empty, CellValues.String),
                                ConstructCell(string.Empty, CellValues.String),
                                ConstructCell(string.Empty, CellValues.String),
                                ConstructCell(string.Empty, CellValues.String),
                                ConstructCell(string.Empty, CellValues.String),
                                ConstructCell(string.Empty, CellValues.String),
                                ConstructCell(string.Empty, CellValues.String),
                                ConstructCell(string.Empty, CellValues.String),
                                ConstructCell(string.Empty, CellValues.String),
                                ConstructCell(string.Empty, CellValues.String),
                                ConstructCell(string.Empty, CellValues.String),
                                ConstructCell(string.Empty, CellValues.String),
                                ConstructCell(string.Empty, CellValues.String),
                                 ConstructCell(string.Empty, CellValues.String),
                                ConstructCell(string.Empty, CellValues.String),
                                ConstructCell(string.Empty, CellValues.String),
                                ConstructCell(string.Empty, CellValues.String),
                                ConstructCell(string.Empty, CellValues.String),
                                ConstructCell(string.Empty, CellValues.String),


                                //
                                ConstructCell(d.Tag61And86GroupCRTs[contIndex]?.AccOwnerInfo86Info ?? string.Empty, CellValues.String),
                                 ConstructCell(d.Tag61And86GroupCRTs[contIndex]?.StatementLine61DOrC ?? string.Empty, CellValues.String),
                                ConstructCell(d.Tag61And86GroupCRTs[contIndex]?.StatementLine61Amount.ToString(), CellValues.String),
                                ConstructCell(d.Tag61And86GroupCRTs[contIndex]?.StatementLine61EntryDate, CellValues.String),
                                ConstructCell(d.Tag61And86GroupCRTs[contIndex].StatementLine61CustomerRef ?? string.Empty, CellValues.String),
                                ConstructCell(d.Tag61And86GroupCRTs[contIndex]?.StatementLine61TrnsactnTypeID ?? string.Empty, CellValues.String),
                                ConstructCell(d.Tag61And86GroupCRTs[contIndex]?.StatementLine61ValueDate.ToShortDateString(), CellValues.String)
                                );

                                sheetData.AppendChild(row);
                            }
                        }

                        worksheetPart.Worksheet.Save();
                        //MessagingCenter.Send(this, "DataExportedSuccessfully");
                    }

                    /*
                    if(await EntityPage.DisplayAlert(StaticVariables.AppName, $"{StaticVariables.DownloadComplete}{Environment.NewLine}{StaticVariables.OpenSavedFileRequest}", StaticVariables.Yes, StaticVariables.No) )
                    {
                        await Launcher.OpenAsync(fileName);
                    }
                    */

                    fileNm = fileName;

                    await EntityPage.DisplayAlert(StaticVariables.AppName, $"{StaticVariables.DownloadComplete}", StaticVariables.Okay);

                    //MessagingCenter.Send<object>(string.Empty, StaticVariables.IndivDownloadCompleteMessage);
                }
            }
            catch (Exception exc)
            {
                await EntityPage.DisplayAlert(StaticVariables.AppErrorTitle, StaticVariables.FileSaveFailedErrorMessage, StaticVariables.Cancel);
            }


            IsBusy = false;
            return fileNm;
        }
        #endregion


        #region Set Property and Notify Property Changed
        #region Set Property
        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion


        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        #endregion


        #region Populate List
        public async Task PopulateListAsync()
        {
            if (!IsInAutoMode) return;

            if (!StaticVariables.DeviceHasInternetAccess())
            {
                await EntityPage.DisplayAlert(StaticVariables.NoInternet, StaticVariables.ConnectToInternet, StaticVariables.Cancel);
                return;
            }

            IsBusy = true;


            var accessToken = StaticVariables.GetStringValueFromUserPrefKey(StaticVariables.JwtTokenPreferenceKey, string.Empty);

            if (string.IsNullOrEmpty(accessToken.Trim()))
            {
                if (await EntityPage.DisplayAlert(StaticVariables.Login, StaticVariables.UserNotLoggedIn, StaticVariables.Login, StaticVariables.Cancel) == true)
                {
                    await EntityPage.Navigation.PushModalAsync(new LoginPage());
                }
                IsBusy = false;
                return;
            }

            var resultFromApi = await StaticApiHelper.GetStoredMT940sAsync(accessToken);

            if (resultFromApi != null)
            {
                MessagingCenter.Send<object>(resultFromApi, StaticVariables.ShareMultipleMT940Message);
            }

            IsBusy = false;
        }
        #endregion
    }
}
