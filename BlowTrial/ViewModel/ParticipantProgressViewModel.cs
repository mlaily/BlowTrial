﻿using BlowTrial.Helpers;
using BlowTrial.Infrastructure.Interfaces;
using BlowTrial.Models;
using BlowTrial.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using BlowTrial.Infrastructure.Extensions;
using System.Windows;
using BlowTrial.Domain.Outcomes;
using MvvmExtraLite.Helpers;
using BlowTrial.Domain.Tables;
using AutoMapper;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
//using System.Windows.Threading;
using System.Windows.Media;
using StatsForAge.DataSets;
using BlowTrial.Infrastructure.Extensions;

namespace BlowTrial.ViewModel
{
    public sealed class ParticipantProgressViewModel : ParticipantListItemViewModel, IDataErrorInfo
    {
        #region Fields
        OutcomeAt28DaysSplitter _outcomeSplitter;
        string _dischargeExplanation;
        ObservableCollection<VaccineViewModel> _allVaccinesAvailable;
        #endregion

        #region Constructors
        public ParticipantProgressViewModel(IRepository repository ,ParticipantProgressModel participant) : base(participant, repository)
        {
            _outcomeSplitter = new OutcomeAt28DaysSplitter(ParticipantProgressModel.OutcomeAt28Days);
            SaveChanges = new RelayCommand(Save, CanSave);
            NewVaccineCmd = new RelayCommand(CreateNewVaccine, CanCreateNewVaccine);

            AttachCollections();
        }

        #endregion

        #region Properties

        ParticipantProgressModel ParticipantProgressModel
        {
            get
            {
                return (ParticipantProgressModel)ParticipantModel;
            }
        }

        public ObservableCollection<VaccineAdministeredViewModel> VaccineVMsAdministered { get; private set; }

        public new ICollection<VaccineAdministered> VaccinesAdministered
        {
            get
            {
                return base.VaccinesAdministered;
            }
            set
            {
                foreach(var va in value)
                {
                    var match = VaccineVMsAdministered.FirstOrDefault(vavm=>vavm.VaccineGiven.Id == va.VaccineId);
                    if (match==null)
                    {
                        VaccineVMsAdministered.Add(new VaccineAdministeredViewModel(
                            new VaccineAdministeredModel
                            {
                                AdministeredTo = ParticipantProgressModel, 
                                Id = va.Id, 
                                AdministeredAtDateTime = va.AdministeredAt, 
                                VaccineGiven = va.VaccineGiven
                            }
                            , AllVaccinesAvailable) { AllowEmptyRecord = false });
                    }
                    else
                    {
                        match.VaccineAdministeredModel.Id = va.Id;
                        match.AdministeredAtDateTime = va.AdministeredAt;
                    }
                }
                base.VaccinesAdministered = value;
            }
        }

        public bool IsParticipantModelChanged { get; internal set; }

        public bool IsVaccineAdminChanged { get; internal set; }

        public override string DisplayName
        {
            get
            {
                return string.Format(Strings.ParticipantUpdateVM_DisplayName, ParticipantProgressModel.Id);
            }
        }

        public string DischargeExplanation
        {
            get
            {
                return _dischargeExplanation ?? (_dischargeExplanation = BlowTrialDataService.GetRandomisingMessage().DischargeExplanation);
            }
        }

        public int CentreId
        {
            get
            {
                return ParticipantProgressModel.CentreId;
            }
        }

        public string Notes
        {
            get
            {
                return ParticipantProgressModel.Notes;
            }
            set
            {
                if (value == ParticipantProgressModel.Notes) { return; }
                ParticipantProgressModel.Notes = value;
                NotifyPropertyChanged("Notes");
            }
        }

        public string PhoneMask
        {
            get { return ParticipantProgressModel.StudyCentre.PhoneMask; }
        }

        public string CGA
        {
            get
            {
                return ParticipantProgressModel.GestAgeBirth.ToCgaString(AgeDays);
            }
        }

        public new int AgeDays
        {
            get
            {
                return base.AgeDays;
            }
            set
            {
                if (base.AgeDays != value)
                {
                    base.AgeDays = value;
                    NotifyPropertyChanged("CGA", "TodayOr28", "DeathLastContactTodayOr28");
                    OutcomeAt28orDischargeOptions.First(o => o.Key == OutcomeAt28DaysOption.InpatientAt28Days).IsEnabled = base.AgeDays >= 28;
                }
            }
        }

        public string MothersName
        {
            get
            {
                return ParticipantProgressModel.MothersName;
            }
            internal set
            {
                if (ParticipantProgressModel.MothersName != value)
                {
                    ParticipantProgressModel.MothersName = value;
                    NotifyPropertyChanged("MothersName");
                }
            }
        }

        public string PhoneNumber
        {
            get
            {
                return ParticipantProgressModel.PhoneNumber ?? Strings.ParticipantUpdateVM_NoPhone;
            }
            internal set
            {
                if (ParticipantProgressModel.PhoneNumber != value)
                {
                    ParticipantProgressModel.PhoneNumber = value;
                    NotifyPropertyChanged("PhoneNumber");
                }
            }
        }

        public int AdmissionWeight
        {
            get
            {
                return ParticipantProgressModel.AdmissionWeight;
            }
            internal set
            {
                if (ParticipantProgressModel.AdmissionWeight != value)
                {
                    ParticipantProgressModel.AdmissionWeight = value;
                    NotifyPropertyChanged("AdmissionWeight");
                }
            }
        }

        public bool? BcgAdverse
        {
            get
            {
                return ParticipantProgressModel.BcgAdverse;
            }
            set
            {
                if (value == ParticipantProgressModel.BcgAdverse) { return; }
                ParticipantProgressModel.BcgAdverse = value;
                IsParticipantModelChanged = true;
                NotifyPropertyChanged("BcgAdverse", "BcgAdverseDetail");
            }
        }
        public string BcgAdverseDetail
        {
            get
            {
                return ParticipantProgressModel.BcgAdverseDetail;
            }
            set
            {
                if (value == ParticipantProgressModel.BcgAdverseDetail) { return; }
                ParticipantProgressModel.BcgAdverseDetail = value;
                IsParticipantModelChanged = true;
                NotifyPropertyChanged("BcgAdverseDetail");
            }
        }
        public bool? BcgPapule
        {
            get
            {
                return ParticipantProgressModel.BcgPapule;
            }
            set
            {
                if (value == ParticipantProgressModel.BcgPapule) { return; }
                ParticipantProgressModel.BcgPapule = value;
                IsParticipantModelChanged = true;
                NotifyPropertyChanged("BcgPapule");
            }
        }
        public int? LastContactWeight
        {
            get
            {
                return ParticipantProgressModel.LastContactWeight;
            }
            set
            {
                if (value == ParticipantProgressModel.LastContactWeight) { return; }
                ParticipantProgressModel.LastContactWeight = value;
                IsParticipantModelChanged = true;
                NotifyPropertyChanged("LastContactWeight", "LastWeightDate");
                CalculateWtCentile();
            }
        }
        public DateTime? LastWeightDate
        {
            get
            {
                return ParticipantProgressModel.LastWeightDate;
            }
            set
            {
                if (value == ParticipantProgressModel.LastWeightDate) { return; }
                ParticipantProgressModel.LastWeightDate = value;
                IsParticipantModelChanged = true;
                NotifyPropertyChanged("LastWeightDate");
                CalculateWtCentile();
            }
        }
        
        public CauseOfDeathOption CauseOfDeath
        {
            get
            {
                return ParticipantProgressModel.CauseOfDeath;
            }
            set
            {
                if (value == ParticipantProgressModel.CauseOfDeath) { return; }
                ParticipantProgressModel.CauseOfDeath = value;
                IsParticipantModelChanged = true;
                NotifyPropertyChanged("CauseOfDeath", "OtherCauseOfDeathDetail");
            }
        }

        public DateTime? DischargeDateTime
        {
            get
            {
                return ParticipantProgressModel.DischargeDateTime;
            }
            internal set
            {
                if (ParticipantProgressModel.DischargeDateTime == value) { return; }
                ParticipantProgressModel.DischargeDateTime = value;
                NotifyPropertyChanged("DischargeDate", "DischargeTime", "DischargeOrEnrolment");
            }
        }

        public DateTime? DischargeDate
        {
            get
            {
                return ParticipantProgressModel.DischargeDate;
            }
            set
            {
                if (ParticipantProgressModel.DischargeDate == value) { return; }
                ParticipantProgressModel.DischargeDate = value;
                IsParticipantModelChanged = true;
                NotifyPropertyChanged("DischargeDate", "DischargeTime", "DischargeOrEnrolment");
            }
        }
        public TimeSpan? DischargeTime
        {
            get { return ParticipantProgressModel.DischargeTime; }
            set
            {
                if (ParticipantProgressModel.DischargeTime == value) { return; }
                ParticipantProgressModel.DischargeTime = value;
                IsParticipantModelChanged = true;
                NotifyPropertyChanged("DischargeTime", "DischargeDate");
            }
        }

        public DateTime? DeathOrLastContactDateTime
        {
            get
            {
                return ParticipantProgressModel.DeathOrLastContactDateTime;
            }
            internal set
            {
                if (ParticipantProgressModel.DeathOrLastContactDateTime == value) { return; }
                ParticipantProgressModel.DeathOrLastContactDateTime = value;
                NotifyPropertyChanged("DeathOrLastContactDate", "DeathOrLastContactTime", "DeathLastContactTodayOr28", "LastWeightDate");
            }
        }

        public DateTime? DeathOrLastContactDate
        {
            get
            {
                return ParticipantProgressModel.DeathOrLastContactDate;
            }
            set
            {
                if (ParticipantProgressModel.DeathOrLastContactDate == value) { return; }
                ParticipantProgressModel.DeathOrLastContactDate = value;
                IsParticipantModelChanged = true;
                NotifyPropertyChanged("DeathOrLastContactDate", "DeathOrLastContactTime", "DeathLastContactTodayOr28", "LastWeightDate");
            }
        }
        public TimeSpan? DeathOrLastContactTime
        {
            get { return ParticipantProgressModel.DeathOrLastContactTime; }
            set
            {
                if (ParticipantProgressModel.DeathOrLastContactTime == value) { return; }
                ParticipantProgressModel.DeathOrLastContactTime = value;
                IsParticipantModelChanged = true;
                NotifyPropertyChanged("DeathOrLastContactTime", "DeathOrLastContactDate");
            }
        }
        public string DeathOrLastContactLabel
        {
            get
            {
                switch (ParticipantProgressModel.IsKnownDead)
                {
                    case null:
                        return Strings.ParticipantUpdateView_Label_LastContactDateTime.ToLabelFormat();
                    case true:
                        return Strings.ParticipantUpdateView_Label_DeathDateTime.ToLabelFormat();
                    default: //false
                        return null;
                }
            }
        }

        public string BcgPapuleLabel
        {
            get
            {
                switch (OutcomeAt28orDischarge)
                {
                    case OutcomeAt28DaysOption.DiedInHospitalBefore28Days:
                        return (Strings.ParticipantUpdateView_Label_BcgInduration + ' ' + Strings.ParticipantUpdateView_Label_Death).ToLabelFormat();
                    case OutcomeAt28DaysOption.InpatientAt28Days:
                        return (Strings.ParticipantUpdateView_Label_BcgInduration + ' ' + Strings.ParticipantUpdateView_Label_28days).ToLabelFormat();
                    case OutcomeAt28DaysOption.DischargedBefore28Days:
                        return (Strings.ParticipantUpdateView_Label_BcgInduration + ' ' + Strings.ParticipantUpdateView_Label_Discharge).ToLabelFormat();
                    default:
                        return null;
                }
            }
        }
        public string WeightLabel
        {
            get 
            {
                switch (OutcomeAt28orDischarge)
                {
                    case OutcomeAt28DaysOption.DiedInHospitalBefore28Days:
                        return (Strings.ParticipantUpdateView_Label_Weight + ' ' + Strings.ParticipantUpdateView_Label_Death).ToLabelFormat();
                    case OutcomeAt28DaysOption.InpatientAt28Days:
                        return (Strings.ParticipantUpdateView_Label_Weight + ' ' + Strings.ParticipantUpdateView_Label_28days).ToLabelFormat();
                    default:
                        if (OutcomeAt28orDischarge!=OutcomeAt28DaysOption.Missing)
                        {
                            return (Strings.ParticipantUpdateView_Label_Weight + ' ' + Strings.ParticipantUpdateView_Label_Discharge).ToLabelFormat();
                        }
                        return null;
                }
            }
        }
        public string OtherCauseOfDeathDetail
        {
            get
            {
                return ParticipantProgressModel.OtherCauseOfDeathDetail;
            }
            set
            {
                if (value == ParticipantProgressModel.OtherCauseOfDeathDetail) { return; }
                ParticipantProgressModel.OtherCauseOfDeathDetail = value;
                IsParticipantModelChanged = true;
                NotifyPropertyChanged("OtherCauseOfDeathDetail");
            }
        }
        public bool DischargedBy28Days
        {
            get 
            {
                return ParticipantProgressModel.OutcomeAt28Days >= OutcomeAt28DaysOption.DischargedBefore28Days;
            }
        }

        public OutcomeAt28DaysOption OutcomeAt28orDischarge
        {
            get
            {
                return _outcomeSplitter.OutcomeAt28orDischarge;
            }
            set
            {
                _outcomeSplitter.OutcomeAt28orDischarge = value;
                if (_outcomeSplitter.OutcomeAt28orDischarge == ParticipantProgressModel.OutcomeAt28Days) { return; }
                OutcomeAt28Days = _outcomeSplitter.OutcomeAt28Days;
                if (OutcomeAt28Days < OutcomeAt28DaysOption.DischargedBefore28Days)
                {
                    ParticipantProgressModel.DischargeDate = null;
                    ParticipantProgressModel.DischargeTime = null;
                    _outcomeSplitter.PostDischargeOutcomeKnown = null;
                    _outcomeSplitter.DiedAfterDischarge = null;
                }
                NotifyPropertyChanged("DischargeDate", "DischargeTime", "OutcomeAt28orDischarge", "BcgPapuleLabel", "PostDischargeOutcomeKnown", "DiedAfterDischarge");
            }
        }
        public OutcomeAt28DaysOption OutcomeAt28Days
        {
            get 
            {
                return ParticipantProgressModel.OutcomeAt28Days;
            }
            internal set 
            {
                if (value == ParticipantProgressModel.OutcomeAt28Days) { return; }
                ParticipantProgressModel.OutcomeAt28Days = value;
                IsParticipantModelChanged = true;
                if (!IsDeathOrLastContactRequired) 
                {
                    ParticipantProgressModel.DeathOrLastContactDate = null;
                    ParticipantProgressModel.DeathOrLastContactTime = null;
                    ParticipantProgressModel.CauseOfDeath = CauseOfDeathOption.Missing;
                }
                NotifyPropertyChanged("OutcomeAt28Days", "DischargedBy28Days", "IsKnownDead", "CauseOfDeath","DeathOrLastContactLabel", "WeightLabel", "IsDeathOrLastContactRequired", "DeathOrLastContactDate", "DeathOrLastContactTime");
            }
        }
        public bool? PostDischargeOutcomeKnown
        {
            get 
            {
                return _outcomeSplitter.PostDischargeOutcomeKnown;
            }
            set
            {
                if (value == _outcomeSplitter.PostDischargeOutcomeKnown) { return; }
                _outcomeSplitter.PostDischargeOutcomeKnown = value;
                NotifyPropertyChanged("PostDischargeOutcomeKnown");
                OutcomeAt28Days = _outcomeSplitter.OutcomeAt28Days;
                if (_outcomeSplitter.PostDischargeFieldsComplete)
                {
                    NotifyPropertyChanged("DiedAfterDischarge");// to remove validation messages
                }
            }
        }
        
        public bool? DiedAfterDischarge
        {
            get
            {
                return _outcomeSplitter.DiedAfterDischarge;
            }
            set
            {
                if (value == _outcomeSplitter.DiedAfterDischarge) { return; }
                _outcomeSplitter.DiedAfterDischarge = value;
                NotifyPropertyChanged("DiedAfterDischarge", "CauseOfDeath", "DeathOrLastContactTime", "DeathOrLastContactDate");
                OutcomeAt28Days = _outcomeSplitter.OutcomeAt28Days;
                if (_outcomeSplitter.PostDischargeFieldsComplete)
                {
                    NotifyPropertyChanged("PostDischargeOutcomeKnown");// to remove validation messages
                }
            }
        }

        public bool IsDeathOrLastContactRequired
        {
            get
            {
                return IsKnownDead == true || PostDischargeOutcomeKnown == false;
            }
        }
        public DateTime DischargeOrEnrolment
        {
            get
            {
                return DischargeDate
                    ?? RegisteredAt;
            }
        }
        public DateTime DeathLastContactTodayOr28
        {
            get
            {
                return DeathOrLastContactDate
                    ?? TodayOr28;
            }
        }
        string _newVaccineName;
        public string NewVaccineName 
        {
            get {return _newVaccineName;}
            set 
            {
                string newVal = value.Trim();
                if (_newVaccineName == newVal) { return; }
                _newVaccineName = newVal;
                NotifyPropertyChanged ("NewVaccineName");
            }
        }

        public DateTime TodayOr28 // such a silly property is for the upper bound of the datepickers
        {
            get
            {
                return new DateTime[]{DateTime.Today, ParticipantProgressModel.Becomes28On}.Min(d=>d);
            }
        }
        private string _wtForAgeCentile;
        public string WtForAgeCentile
        {
            get
            {
                return _wtForAgeCentile;
            }
            private set
            {
                if (value == _wtForAgeCentile) { return; }
                _wtForAgeCentile = value;
                NotifyPropertyChanged("WtForAgeCentile");
            }
        }
        #endregion

        #region Listbox options
        IEnumerable<KeyValuePair<CauseOfDeathOption, string>> _CauseOfDeathOptions;
        public IEnumerable<KeyValuePair<CauseOfDeathOption, string>> CauseOfDeathOptions
        {
            get
            {
                return _CauseOfDeathOptions 
                    ?? (_CauseOfDeathOptions = EnumToListOptions<CauseOfDeathOption>());
            }
        }
        IEnumerable<SelectableItem<OutcomeAt28DaysOption>> _outcomeAt28orDischargeOptions;
        public IEnumerable<SelectableItem<OutcomeAt28DaysOption>> OutcomeAt28orDischargeOptions
        {
            get
            {
                if (_outcomeAt28orDischargeOptions==null)
                {
                    bool is28daysOld = AgeDays >= 28;
                    _outcomeAt28orDischargeOptions = (from o in EnumToListOptions<OutcomeAt28DaysOption>()
                                                      where o.Key <= OutcomeAt28DaysOption.DischargedBefore28Days
                                                      select new SelectableItem<OutcomeAt28DaysOption>(o.Key, o.Value)
                                                      {
                                                        IsEnabled = o.Key != OutcomeAt28DaysOption.InpatientAt28Days || is28daysOld
                                                      }).ToArray();
                }
                return _outcomeAt28orDischargeOptions;
            }
        }

        ObservableCollection<VaccineViewModel> AllVaccinesAvailable
        {
            get
            {
                if (_allVaccinesAvailable == null)
                {
                    var allVaccines = _repository.Vaccines.ToList()
                        .Select(v=>new VaccineViewModel(v)).ToList();
                    allVaccines.Insert(0, new VaccineViewModel(null));
                    _allVaccinesAvailable = new ObservableCollection<VaccineViewModel>(allVaccines);
                }
                return _allVaccinesAvailable;
            }
        }
        KeyValuePair<bool?, string>[] _bcgPapuleOptions;
        public KeyValuePair<bool?, string>[] BcgPapuleOptions
        {
            get
            {
                return _bcgPapuleOptions ?? (_bcgPapuleOptions = CreateBoolPairs(nullString: Strings.ParticipantUpdateVM_Option_BcgPapuleNull));
            }
        }

        KeyValuePair<bool?, string>[] _requiredBoolOptions;
        public KeyValuePair<bool?, string>[] RequiredBoolOptions
        {
            get
            {
                return _requiredBoolOptions ?? (_requiredBoolOptions = CreateBoolPairs());
            }
        }
        #endregion

        #region IDataErrorInfo Members

        string IDataErrorInfo.Error { get { return null; } }

        string IDataErrorInfo.this[string propertyName]
        {
            get 
            { 
                string returnVal = this.GetValidationError(propertyName);
                CommandManager.InvalidateRequerySuggested();
                return returnVal;
            }
        }

        #endregion // IDataErrorInfo Members

        #region Methods
        void CalculateWtCentile()
        {
            if (LastWeightDate == null || LastContactWeight==null)
            {
                WtForAgeCentile = string.Empty;
            }
            else
            {
                var weightData = new UKWeightData(); 
                //note addDays is just to get roughly the mean time of weight
                WtForAgeCentile = string.Format(Strings.NewPatientVM_Centile,
                    weightData.CumSnormForAge((double)LastContactWeight.Value / 1000, (LastWeightDate.Value.AddDays(0.5) - DateTimeBirth.Date).TotalDays, ParticipantProgressModel.IsMale, ParticipantProgressModel.GestAgeBirth));
            }
        }

        void AttachCollections()
        {
            if (ParticipantProgressModel.VaccineModelsAdministered == null)
            {
                ParticipantProgressModel.VaccineModelsAdministered = Mapper.Map<List<VaccineAdministeredModel>>((from r in _repository.VaccinesAdministered
                         where r.ParticipantId == ParticipantProgressModel.Id
                         select r).ToList());
                
            }
            if (ParticipantProgressModel.StudyCentre == null)
            {
                ParticipantProgressModel.StudyCentre = _repository.FindStudyCentre(ParticipantProgressModel.CentreId);
            }
            if (VaccineVMsAdministered != null)
            {
                VaccineVMsAdministered.Clear(); // remove event handlers
            }
            else
            {
                VaccineVMsAdministered = new ObservableCollection<VaccineAdministeredViewModel>();
                VaccineVMsAdministered.CollectionChanged += VaccinesAdministered_CollectionChanged;
            }

            foreach (VaccineAdministeredModel model in ParticipantProgressModel.VaccineModelsAdministered)
            {
                VaccineAdministeredViewModel vm = new VaccineAdministeredViewModel (model, AllVaccinesAvailable);
                VaccineVMsAdministered.Add(vm);
            }
            VaccineVMsAdministered.Add(NewVaccineAdministeredViewModel());
        }
        VaccineAdministeredViewModel NewVaccineAdministeredViewModel()
        {
            var returnVar = new VaccineAdministeredViewModel(
                new VaccineAdministeredModel
                {
                    AdministeredTo = ParticipantProgressModel
                }
                , AllVaccinesAvailable) { AllowEmptyRecord = true };
            returnVar.PropertyChanged += NewVaccineAdminVm_PropertyChanged;
            return returnVar;
        }
        private void VaccinesAdministered_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (VaccineAdministeredViewModel v in e.OldItems)
                {
                    v.PropertyChanged -= NewVaccineAdminVm_PropertyChanged;
                    v.PropertyChanged -= VaccineAdminVm_PropertyChanged;

                    var item = ParticipantProgressModel.VaccineModelsAdministered.FirstOrDefault(va=>va == v.VaccineAdministeredModel);
                    if (item!=null)
                    {
                        ParticipantProgressModel.VaccineModelsAdministered.Remove(item);
                        if (item.VaccineGiven != null)
                        {
                            AllVaccinesAvailable.First(a => a.Vaccine == item.VaccineGiven).IsGivenToThisPatient = false;
                        }
                        IsVaccineAdminChanged = true;
                    }
                }
            }
            if (e.NewItems != null)
            {
                foreach (VaccineAdministeredViewModel v in e.NewItems)
                {
                    v.PropertyChanged += VaccineAdminVm_PropertyChanged;
                }
            }
        }
        void VaccineAdminVm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "AdministeredAtDate" || e.PropertyName == "AdministeredAtTime" || e.PropertyName == "SelectedVaccine")
            {
                IsVaccineAdminChanged = true;
            }
        }
        private void NewVaccineAdminVm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "AdministeredAtDate" || e.PropertyName == "AdministeredAtTime" || e.PropertyName == "SelectedVaccine")
            {
                var newVaccineAdminVm = (VaccineAdministeredViewModel)sender;
                newVaccineAdminVm.AllowEmptyRecord = newVaccineAdminVm.IsEmpty;
                if (!newVaccineAdminVm.IsEmpty && newVaccineAdminVm.IsValid())
                {
                    newVaccineAdminVm.AllowEmptyRecord = false;
                    ParticipantProgressModel.VaccineModelsAdministered.Add(newVaccineAdminVm.VaccineAdministeredModel);
                    newVaccineAdminVm.PropertyChanged -= NewVaccineAdminVm_PropertyChanged;
                    VaccineVMsAdministered.Add(NewVaccineAdministeredViewModel());
                }
            }
        }

        #endregion

        #region ICommands
        public ICommand SaveChanges { get; private set; }
        public EventHandler OnSave;
        bool CanSave(object param)
        {
            return (IsParticipantModelChanged && WasValidOnLastNotify) || (IsVaccineAdminChanged  && VaccineVMsAdministered.All(v => v.IsValid()));
        }
        void Save(object param)
        {
            if (IsParticipantModelChanged)
            {
                _repository.UpdateParticipant(
                    id : ParticipantProgressModel.Id,
                    causeOfDeath : ParticipantProgressModel.CauseOfDeath,
                    otherCauseOfDeathDetail: ParticipantProgressModel.OtherCauseOfDeathDetail,
                    bcgAdverse : ParticipantProgressModel.BcgAdverse,
                    bcgAdverseDetail: ParticipantProgressModel.BcgAdverseDetail,
                    bcgPapule : ParticipantProgressModel.BcgPapule,
                    lastContactWeight : ParticipantProgressModel.LastContactWeight,
                    lastWeightDate : ParticipantProgressModel.LastWeightDate,
                    dischargeDateTime : ParticipantProgressModel.DischargeDateTime,
                    deathOrLastContactDateTime : ParticipantProgressModel.DeathOrLastContactDateTime,
                    outcomeAt28Days : ParticipantProgressModel.OutcomeAt28Days,
                    notes : ParticipantProgressModel.Notes
                );
                DataRequired = ParticipantProgressModel.RecalculateDataRequired();
                IsParticipantModelChanged = false;
            }
            if (IsVaccineAdminChanged)
            {
                var vas = ParticipantProgressModel.VaccineModelsAdministered.Select
                    (v => new VaccineAdministered 
                    {
                        Id = v.Id,
                        AdministeredAt = v.AdministeredAtDateTime.Value,
                        VaccineId = v.VaccineGiven.Id
                    });
                _repository.AddOrUpdateVaccinesFor(ParticipantProgressModel.Id, vas);
                //use fact only 1 vaccine allowed per participant
                foreach (var vm in ParticipantProgressModel.VaccineModelsAdministered)
                {
                    if (vm.Id == 0)
                    {
                        vm.Id = vas.First(v=>v.VaccineId == vm.VaccineGiven.Id).Id;
                    }
                }
                IsVaccineAdminChanged = false;
            }
            if (OnSave != null)
            {
                OnSave(this, new EventArgs());
            }
        }
        public ICommand NewVaccineCmd { get; private set; }
        bool CanCreateNewVaccine(object param)
        {
            return !string.IsNullOrWhiteSpace(NewVaccineName) && GetValidationError("NewVaccineName") == null;
        }
        void CreateNewVaccine(object param)
        {
            Vaccine vax = new Vaccine { Name = NewVaccineName };
            _repository.Add(vax);
            AllVaccinesAvailable.Add(new VaccineViewModel(vax));
        }
        #endregion

        #region Window Event Handlers

        internal void OnClosingWindow(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !OkToProceed();
        }

        #endregion

        #region Methods

        public bool OkToProceed()
        {
            if (IsParticipantModelChanged || IsVaccineAdminChanged)
            {
                string title;
                string msg;
                MessageBoxButton buttonOptions;
                if (IsValid())
                {
                    title = Strings.ParticipantUpdateVM_Confirm_SaveChanges_Title;
                    msg = Strings.ParticipantUpdateVM_Confirm_SaveChanges;
                    buttonOptions = MessageBoxButton.YesNoCancel;
                }
                else
                {
                    title = Strings.ParticipantUpdateVM_Confirm_Close_Title;
                    msg = Strings.ParticipantUpdateVM_Confirm_Close;
                    buttonOptions = MessageBoxButton.OKCancel;
                }
                MessageBoxResult result = MessageBox.Show(
                    msg,
                    title,
                    buttonOptions,
                    MessageBoxImage.Question);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        Save(null);
                        break;
                    case MessageBoxResult.Cancel:
                        return false;
                    default: //OK in Proceed ? OK/Cancel and No In Close without saving? yes/no/cancel
                        CancelChanges();
                        break;
                }
            }
            // Yes, OK or No
            return true; 
        }

        public EventHandler OnCancel;

        void CancelChanges()
        {
            string oldName = ParticipantProgressModel.Name;
            ParticipantModel = Mapper.Map<ParticipantProgressModel>(
                            _repository.Participants.Include("VaccinesAdministered").Include("VaccinesAdministered.VaccineGiven")
                                .First(p => p.Id == ParticipantProgressModel.Id));
            IsParticipantModelChanged = IsVaccineAdminChanged = false;
            _outcomeSplitter = new OutcomeAt28DaysSplitter(ParticipantProgressModel.OutcomeAt28Days);
            AttachCollections();
            if (oldName != ParticipantProgressModel.Name)
            {
                NotifyPropertyChanged("Name");
            }
            if (OnCancel !=null)
            {
                OnCancel(this, new EventArgs());
            }
        }

        #endregion

        #region AgeTimer

        static TimeSpan IntervalToSameTime(DateTime orginalDateTime)
        {
            var returnVar = DateTime.Now.TimeOfDay - orginalDateTime.TimeOfDay;
            if (returnVar.TotalHours < 0)
            {
                returnVar += TimeSpan.FromDays(1);
            }
            return returnVar;
        }

        #endregion

        #region Validation

        /// <summary>
        /// Returns true if this object has no validation errors.
        /// </summary>
        public override bool IsValid()
        {
            bool returnVal = ParticipantProgressModel.IsValid() && !ValidatedProperties.Any(v => this.GetValidationError(v) != null);
            CommandManager.InvalidateRequerySuggested();
            return returnVal;
            
        }

        static readonly string[] ValidatedProperties = 
        { 
            "DiedAfterDischarge"
        };

        public string GetValidationError(string propertyName)
        {
            switch (propertyName)
            {
                case "OutcomeAt28orDischarge":
                    propertyName = "OutcomeAt28Days";
                    break;
                case "DiedAfterDischarge":
                case "PostDischargeOutcomeKnown":
                    return ValidatePostDischargeOutcomes();
                case "NewVaccineName":
                    return ValidateNewVaccineName();
            }
            return ((IDataErrorInfo)ParticipantProgressModel)[propertyName];
        }
        string ValidateNewVaccineName()
        {
            if (!string.IsNullOrEmpty(NewVaccineName) && _repository.Vaccines.Any(v => v.Name == NewVaccineName))
            {
                return Strings.CreateNewVaccine_Duplicate;
            }
            return null;
        }
        string ValidatePostDischargeOutcomes()
        {
            if (PostDischargeOutcomeKnown.HasValue && !DiedAfterDischarge.HasValue)
            {
                return string.Format(Strings.ParticipantUpdateVM_Error_DiedAfterDischargeRequired,
                    PostDischargeOutcomeKnown.Value
                        ?Strings.ParticipantUpdateVM_Error_Known
                        :Strings.ParticipantUpdateVM_Error_Likely);
            }
            if (DiedAfterDischarge.HasValue && !PostDischargeOutcomeKnown.HasValue)
            {
                return string.Format(Strings.ParticipantUpdateVM_Error_PostDischargeOutcomeRequired,
                    DiedAfterDischarge.Value
                        ? Strings.ParticipantUpdateVM_Error_Death
                        : Strings.ParticipantUpdateVM_Error_Survival);
            }
            return null;
        }

        #endregion //Validation

        #region finaliser

        #endregion
    }
}