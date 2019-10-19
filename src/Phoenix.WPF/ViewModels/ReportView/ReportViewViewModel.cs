namespace Phoenix.WPF.ViewModels.ReportView {
    using System;
    using System.Collections.Generic;
    using System.IO.Packaging;
    using System.Threading;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Markup;

    using CodeReason.Reports;

    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Threading;

    using Phoenix.Domain;
    using Phoenix.Infrastructure.Extensions;
    using Phoenix.Resources;
    using Phoenix.WPF.CustomControls;
    using Phoenix.WPF.Models;
    using Phoenix.WPF.Properties;

    using Utils = Phoenix.WPF.Utils;

    public class ReportViewViewModel<T> : ViewModelBase {
        private readonly string _reportFileName;
        private IList<T> _items;

        protected ReportViewViewModel(string reportFileName) {
            _reportFileName = reportFileName;
            ReportUIModel = new ReportUIModel();
        }

        public ReportUIModel ReportUIModel { set; get; }

        public IList<T> Items {
            set {
                _items = value;
                if (!IsInDesignMode)
                    ShowReportAsync();
            }
        }

        private void ShowReport() {
            ReportUIModel.IsBusyIndicatorHidden = false;
            try {
                Utils.EnsureCulture();
                var flowDocument =
                    (FlowDocument)
                    Application.LoadComponent(
                        new Uri(string.Format("Templates/{0}.{1}.xaml", _reportFileName, Settings.Default.Culture),
                                UriKind.Relative));

                var temp = new WindowBase(false, false, false);
                flowDocument.FontFamily = temp.FontFamily;
                flowDocument.FlowDirection = temp.FlowDirection;

                var reportDocument = new ReportDocument {
                                                            XamlData = XamlWriter.Save(flowDocument),
                                                            XpsCompressionOption = CompressionOption.SuperFast
                                                        };

                var data = new ReportData();

                data.ReportDocumentValues.Add("ReportedOn",
                                              string.Format(ReportPreviewResources.ReportGeneratingDate,
                                                            DateTime.Now.ToLocalized()));

                data.ReportDocumentValues.Add("ReportedBy",
                                              string.Format(ReportPreviewResources.ReporterFormat,
                                                            AppContext.Instanse.User.UserName,
                                                            AppContext.Instanse.User.Name ??
                                                            ReportPreviewResources.NoName));

                data.ReportDocumentValues.Add("Slogan", ReportPreviewResources.Slogan);

                data.ReportDocumentValues.Add("ReportTitle", Settings.Default.ReportTitle);
                data.ReportDocumentValues.Add("ReportDescription", Settings.Default.ReportDescription);

                data.DataTables.Add(_items.ToDataTable());

                var xps = reportDocument.CreateXpsDocument(data);

                DispatcherHelper.CheckBeginInvokeOnUI(() => ReportUIModel.Document = xps.GetFixedDocumentSequence());
                /*} catch (Exception) {*/
            } finally {
                ReportUIModel.IsBusyIndicatorHidden = true;
            }
        }

        private void ShowReportAsync() {
            var thread = new Thread(ShowReport);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }
    }
}