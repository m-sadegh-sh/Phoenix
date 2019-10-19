namespace Phoenix.Domain.PropStatusChanges {
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Phoenix.Domain.LabProps;
    using Phoenix.Domain.Logs;
    using Phoenix.Infrastructure;

    public class PropStatusChangesService : ServiceBase<PropStatusChange> {
        public static PropStatusChangesService Instanse {
            get { return new PropStatusChangesService(); }
        }

        private static PropStatusChange Get(Guid propID, DateTime reportDate) {
            return
                PropStatusChangesRepository.Instanse.Get(new PropStatusChange {PropID = propID, ReportedOn = reportDate});
        }

        public override bool Insert(PropStatusChange propStatusChange) {
            try {
                if (Get(propStatusChange.PropID, propStatusChange.ReportedOn) == null) {
                    PropStatusChangesRepository.Instanse.InsertOrUpdateAndSubmit(propStatusChange);
                    LogsService.Instanse.Insert(new Log {
                                                            HostTable = (short) HostTable.PropStatusChanges,
                                                            Details =
                                                                Log.PropStatusChangeDetailer(propStatusChange, ActionType.Created)
                                                        });
                    if (propStatusChange.Type == (short) ReportType.Used ||
                        propStatusChange.Type == (short) ReportType.DeliveredToRepository)
                        LabPropsService.Instanse.Remove(propStatusChange.PropID);
                    return true;
                }
                propStatusChange.LastModifiedOn = DateTime.Now;
                PropStatusChangesRepository.Instanse.InsertOrUpdateAndSubmit(propStatusChange);
                LogsService.Instanse.Insert(new Log {
                                                        HostTable = (short) HostTable.PropStatusChanges,
                                                        Details =
                                                            Log.PropStatusChangeDetailer(propStatusChange, ActionType.Modified)
                                                    });
                return true;
            } catch (Exception exception) {
                Logger.Write(exception);
                return false;
            }
        }

        public override IList<PropStatusChange> Search(Func<PropStatusChange, bool> func) {
            return GetAll().Where(func).ToList();
        }

        public override IList<PropStatusChange> GetAll() {
            return PropStatusChangesRepository.Instanse.GetAll();
        }

        public PropStatusChange GetLatest(Guid propID) {
            var status =
                GetAll().Where(propStatusChange => propStatusChange.PropID == propID).OrderBy(
                    propStatusChange => propStatusChange.ReportedOn).LastOrDefault();
            if (status == null) {
                status = new PropStatusChange {PropID = propID, Type = (short) ReportType.Free};
                Insert(status);
            }
            return status;
        }

        public override bool Remove(PropStatusChange propStatus) {
            try {
                if (propStatus != null) {
                    LogsService.Instanse.Insert(new Log {
                                                            HostTable = (short) HostTable.PropStatusChanges,
                                                            Details = Log.PropStatusChangeDetailer(propStatus, ActionType.Removed)
                                                        });
                    PropStatusChangesRepository.Instanse.DeleteAndSubmit(propStatus);
                    return true;
                }
                return false;
            } catch (Exception exception) {
                Logger.Write(exception);
                return false;
            }
        }
    }
}