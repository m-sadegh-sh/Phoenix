namespace Phoenix.Domain.LabProps {
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Phoenix.Domain.Labs;
    using Phoenix.Domain.Logs;
    using Phoenix.Domain.Props;
    using Phoenix.Infrastructure;

    public class LabPropsService : ServiceBase<LabProp> {
        public static LabPropsService Instanse {
            get { return new LabPropsService(); }
        }

        public override bool Insert(LabProp labProp) {
            try {
                if (Get(labProp) == null) {
                    LabPropsRepository.Instanse.InsertOrUpdateAndSubmit(labProp);
                    LogsService.Instanse.Insert(new Log {
                                                            HostTable = (short) HostTable.LabProps,
                                                            Details = Log.LabPropDetailer(labProp, ActionType.Created)
                                                        });
                    return true;
                }
                LabPropsRepository.Instanse.InsertOrUpdateAndSubmit(labProp);
                LogsService.Instanse.Insert(new Log {
                                                        HostTable = (short) HostTable.LabProps,
                                                        Details = Log.LabPropDetailer(labProp, ActionType.Modified)
                                                    });
                return true;
            } catch (Exception exception) {
                Logger.Write(exception);
                return false;
            }
        }

        public override IList<LabProp> Search(Func<LabProp, bool> func) {
            return GetAll().Where(func).ToList();
        }

        public override IList<LabProp> GetAll() {
            return LabPropsRepository.Instanse.GetAll();
        }

        private static LabProp Get(LabProp labProp) {
            return LabPropsRepository.Instanse.Get(labProp);
        }

        public override bool Remove(LabProp labProp) {
            try {
                labProp = Get(labProp);
                if (labProp != null) {
                    LogsService.Instanse.Insert(new Log {
                                                            HostTable = (short) HostTable.LabProps,
                                                            Details = Log.LabPropDetailer(labProp, ActionType.Removed)
                                                        });
                    LabPropsRepository.Instanse.DeleteAndSubmit(labProp);
                    return true;
                }
                return false;
            } catch (Exception exception) {
                Logger.Write(exception);
                return false;
            }
        }

        public void Remove(Guid propID) {
            try {
                var labProps = LabPropsRepository.Instanse.GetAll(labProp => labProp.PropID == propID);
                if (labProps.Count > 0) {
                    foreach (var labProp in labProps)
                        Remove(labProp);
                }
            } catch (Exception exception) {
                Logger.Write(exception);
            }
        }

        public static IList<LabProp> GetAll(Guid? labID, DateTime? assignedOnLowerBound, DateTime? assignedOnUpperBound,
                                            bool assignedOnOutside) {
            assignedOnLowerBound = assignedOnLowerBound.HasValue
                                       ? assignedOnLowerBound.Value.AddDays(-1)
                                       : DateTime.MinValue;
            assignedOnUpperBound = assignedOnUpperBound.HasValue
                                       ? assignedOnUpperBound.Value.AddDays(1)
                                       : DateTime.MaxValue;
            if (assignedOnOutside) {
                return (from props in PropsService.Instanse.GetAll()
                        join labProps in
                            LabPropsRepository.Instanse.GetAll(
                                labProp => !(labID.HasValue && labID != Guid.Empty) || labProp.LabID == labID) on
                            props.PropID equals labProps.PropID
                        join labs in LabsRepository.Instanse.GetAll() on labProps.LabID equals labs.LabID
                        where labProps.AssignedOn <= assignedOnLowerBound && labProps.AssignedOn >= assignedOnUpperBound
                        select
                            new LabProp {
                                            StringLabID = labs.Name,
                                            StringPropNo = props.StringPropNo,
                                            StringSerialNo = props.StringSerialNo,
                                            StringPropID = props.Name,
                                            StringStatus = props.StringStatus,
                                            AssignedOn = labProps.AssignedOn
                                        }).OrderByDescending(labProp => labProp.AssignedOn).ToList();
            }
            return (from props in PropsService.Instanse.GetAll()
                    join labProps in
                        LabPropsRepository.Instanse.GetAll(
                            labProp => !(labID.HasValue && labID != Guid.Empty) || labProp.LabID == labID) on
                        props.PropID equals labProps.PropID
                    join labs in LabsRepository.Instanse.GetAll() on labProps.LabID equals labs.LabID
                    where labProps.AssignedOn >= assignedOnLowerBound && labProps.AssignedOn <= assignedOnUpperBound
                    select
                        new LabProp {
                                        StringLabID = labs.Name,
                                        StringPropNo = props.StringPropNo,
                                        StringSerialNo = props.StringSerialNo,
                                        StringPropID = props.Name,
                                        StringStatus = props.StringStatus,
                                        AssignedOn = labProps.AssignedOn
                                    }).OrderByDescending(labProp => labProp.AssignedOn).ToList();
        }

        public static DateTime? GetMaxAssignedOn() {
            var labProps = LabPropsRepository.Instanse.GetAll();
            return labProps.Count > 0 ? (DateTime?) labProps.Max(labProp => labProp.AssignedOn) : null;
        }

        public static DateTime? GetMinAssignedOn() {
            var labProps = LabPropsRepository.Instanse.GetAll();
            return labProps.Count > 0 ? (DateTime?) labProps.Min(labProp => labProp.AssignedOn) : null;
        }
    }
}