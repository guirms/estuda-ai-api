using AutoMapper;
using Domain.Dto_s.Machine;
using Domain.Dto_s.Scheduling;
using Domain.Models;
using Domain.Models.Enums.Batch;
using Domain.Models.Enums.Egg;
using Domain.Objects.Dto_s.Asset;
using Domain.Objects.Dto_s.Egg;
using Domain.Objects.Dto_s.Scheduling;
using Domain.Objects.Dto_s.User;
using Domain.Objects.Requests.Batch;
using Domain.Objects.Requests.Customer;
using Domain.Objects.Requests.Machine;
using Domain.Objects.Requests.Plant;
using Domain.Objects.Requests.User;
using Domain.Objects.Responses.Asset;
using Domain.Objects.Responses.Batch;
using Domain.Objects.Responses.Customer;
using Domain.Objects.Responses.Machine;
using Domain.Utils.Helpers;
using Domain.Utils.Languages;
using System.Globalization;

namespace Application.AutoMapper
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            MachineScheduleMap();
            MachineOperationMap();
            EggMap();
            ShiftMap();
            ScheduledStopMap();
            BoxesQuantityMap();
            UserMap();
            CustomerMap();
            BatchMap();
            PlantMap();
            AssetMap();
            ProductMap();
            LayoutMap();
        }

        #region Machine schedule

        public void MachineScheduleMap()
        {
            CreateMap<MachineSchedule, ProductionTimeDto>();
        }

        #endregion

        #region Machine operation

        private void MachineOperationMap()
        {
            CreateMap<MachineOperationDto, ProductionSchedule>()
                .ForMember(p => p.MachineStatusName, opts => opts.MapFrom(m => Translator.Translate(m.MachineStatus.GetName())))
                .ForMember(p => p.StartTime, opts => opts.MapFrom(m => m.StartTime.ToStringTime("hh\\:mm\\:ss")))
                .ForMember(p => p.EndTime, opts => opts.MapFrom(m => m.EndTime.ToStringTime("hh\\:mm\\:ss")));

            CreateMap<MachineOperationDto, MachineOperation>()
                .ForMember(p => p.EggQuantities, opts => opts.MapFrom(m => m.EggQuantitiesDto));

            CreateMap<MachineOperation, MachineOperationDto>()
               .ForMember(p => p.EggQuantitiesDto, opts => opts.MapFrom(m => m.EggQuantities));

            CreateMap<MachineOperation, MachineOperationDataDto>()
               .ForMember(p => p.EggQuantitiesDto, opts => opts.MapFrom(m => m.EggQuantities));
        }

        #endregion

        #region Egg
        private void EggMap()
        {
            CreateMap<WeightQuantity, EggQuantity>()
                .ForMember(e => e.Type, opts => opts.MapFrom(w => EEggQuantityType.Weight));

            CreateMap<ProductionQuantity, EggQuantity>()
                .ForMember(e => e.Type, opts => opts.MapFrom(p => EEggQuantityType.Production));

            CreateMap<CrackedQuantity, EggQuantity>()
                .ForMember(e => e.Type, opts => opts.MapFrom(c => EEggQuantityType.Cracked));

            CreateMap<EggQuantity, EggQuantityDto>();

            CreateMap<EggCategory, EggCategoryDto>();
        }

        #endregion

        #region Shift

        private void ShiftMap()
        {
            CreateMap<ShiftRequest, DefaultTimeDto>()
                .ForMember(d => d.StartTime, opts => opts.MapFrom(s => TimeSpan.Parse(s.StartTime)))
                .ForMember(d => d.EndTime, opts => opts.MapFrom(s => TimeSpan.Parse(s.EndTime)));

            CreateMap<Shift, DefaultTimeDto>()
                .ForMember(d => d.StartTime, opts => opts.MapFrom(s => s.StartTime))
                .ForMember(d => d.EndTime, opts => opts.MapFrom(s => s.EndTime));

            CreateMap<ShiftRequest, Shift>()
                .ForMember(s => s.StartTime, opts => opts.MapFrom(s => TimeSpan.Parse(s.StartTime)))
                .ForMember(s => s.EndTime, opts => opts.MapFrom(s => TimeSpan.Parse(s.EndTime)))
                .ForMember(s => s.InsertedAt, opts => opts.MapFrom(s => DateTime.Now));

            CreateMap<ScheduledStopRequest, DefaultTimeDto>()
                .ForMember(d => d.StartTime, opts => opts.MapFrom(s => TimeSpan.Parse(s.StartTime)))
                .ForMember(d => d.EndTime, opts => opts.MapFrom(s => TimeSpan.Parse(s.EndTime)));

            CreateMap<ScheduledStopRequest, ScheduledStop>()
                .ForMember(s => s.StartTime, opts => opts.MapFrom(s => TimeSpan.Parse(s.StartTime)))
                .ForMember(s => s.EndTime, opts => opts.MapFrom(s => TimeSpan.Parse(s.EndTime)))
                .ForMember(s => s.InsertedAt, opts => opts.MapFrom(s => DateTime.Now));

            CreateMap<ShiftDto, Shift>();
        }

        #endregion

        #region Scheduled stop

        private void ScheduledStopMap()
        {
            CreateMap<ScheduledStopDto, ScheduledStop>();
        }

        #endregion

        #region Boxes quantity

        private void BoxesQuantityMap()
        {
            CreateMap<GeneralProductionData, BoxesQuantity>()
                .ForMember(b => b.BoxQuantity, opts => opts.MapFrom(g => g.BoxQuantity != "-" ? ConvertBoxQuantity(g.BoxQuantity) : 0));
        }

        #endregion

        #region User

        private void UserMap()
        {
            CreateMap<UserRequest, User>()
               .ForMember(u => u.Key, opts => opts.MapFrom(u => u.UserKey))
               .ForMember(u => u.InsertedAt, opts => opts.MapFrom(u => DateTime.Now));

            CreateMap<User, UserInfo>();

            CreateMap<User, UserResultsResponse>()
               .ForMember(u => u.Document, opts => opts.MapFrom(u => u.Document.ToDocument()));
        }

        #endregion

        #region Customer

        private void CustomerMap()
        {
            CreateMap<CustomerRequest, Customer>()
               .ForMember(c => c.InsertedAt, opts => opts.MapFrom(c => DateTime.Now));

            CreateMap<Customer, CustomerTableData>()
              .ForMember(c => c.IsSelected, opts => opts.MapFrom(c => c.BatchStatus == EBatchStatus.Started))
              .ForMember(c => c.Cnpj, opts => opts.MapFrom(c => c.Cnpj.ToCnpj()))
              .ForMember(c => c.BatchStatusValue, opts => opts.MapFrom(c => c.BatchStatus))
              .ForMember(c => c.BatchStatusText, opts => opts.MapFrom(c => Translator.Translate(c.BatchStatus.GetName())));

            CreateMap<Customer, CustomerToFilterResponse>();
        }

        #endregion

        #region Batch

        private void BatchMap()
        {
            CreateMap<BatchRequest, BatchInfoResponse>();
        }

        #endregion

        #region Plant

        private void PlantMap()
        {
            CreateMap<SavePlantRequest, Plant>()
                 .ForMember(p => p.InsertedAt, opts => opts.MapFrom(s => DateTime.Now));

            CreateMap<UpdatePlantRequest, Plant>()
                 .ForMember(p => p.UpdatedAt, opts => opts.MapFrom(u => DateTime.Now))
                 .ForAllMembers(opt => opt.Condition(src => src != null));

            CreateMap<Plant, PlantToTableResponse>()
                .ForMember(p => p.Cnpj, opts => opts.MapFrom(p => p.Cnpj.ToCnpj()))
                .ForMember(p => p.ZipCode, opts => opts.MapFrom(p => p.ZipCode.ToZipCode()));

            CreateMap<Plant, PlantToFilterData>();

            CreateMap<Plant, PlantSchema>()
                .ForMember(p => p.PlantId, opts => opts.MapFrom(p => p.PlantId))
                .ForMember(p => p.UserId, opts => opts.MapFrom(p => p.UserId))
                .ForMember(p => p.AssetSchema, opts => opts.MapFrom(p => p.Assets!.FirstOrDefault()));
        }

        #endregion

        #region Asset

        private void AssetMap()
        {
            CreateMap<SaveAssetRequest, Asset>()
                 .ForMember(a => a.InsertedAt, opts => opts.MapFrom(s => DateTime.Now));

            CreateMap<UpdateAssetRequest, Asset>()
                 .ForMember(a => a.UpdatedAt, opts => opts.MapFrom(u => DateTime.Now))
                 .ForAllMembers(opts => opts.Condition(src => src != null));

            CreateMap<Asset, AssetToTableResponse>()
                .ForMember(a => a.HasFeedback, opts => opts.MapFrom(a => a.HasFeedback ? 1 : 0));

            CreateMap<Asset, AssetToFilterResponse>();

            CreateMap<Asset, AssetSchema>()
                .ForMember(a => a.ProductSchema, opts => opts.MapFrom(a => a.Products));

            CreateMap<Asset, AssetAuthInfo>();
        }

        #endregion

        #region Product

        private void ProductMap()
        {
            CreateMap<SaveProductRequest, Product>()
                .ForMember(p => p.InsertedAt, opts => opts.MapFrom(s => DateTime.Now));

            CreateMap<UpdateProductRequest, Product>()
                 .ForMember(p => p.UpdatedAt, opts => opts.MapFrom(u => DateTime.Now))
                 .ForAllMembers(opt => opt.Condition(src => src != null));

            CreateMap<Product, ProductToTableResponse>();

            CreateMap<Product, ProductSchema>()
                .ForMember(p => p.LayoutSchema, opts => opts.MapFrom(p => p.Layouts));
        }

        #endregion

        #region Layout

        private void LayoutMap()
        {
            CreateMap<SaveLayoutRequest, Layout>()
                .ForMember(l => l.InsertedAt, opts => opts.MapFrom(s => DateTime.Now));

            CreateMap<UpdateLayoutRequest, Layout>()
                 .ForMember(l => l.UpdatedAt, opts => opts.MapFrom(u => DateTime.Now))
                 .ForAllMembers(opt => opt.Condition(src => src != null));

            CreateMap<Layout, LayoutToTableResponse>();

            CreateMap<Layout, LayoutSchema>();
        }

        #endregion

        #region Private methods

        private static double ConvertBoxQuantity(string boxQuantity) => double.Parse(boxQuantity.Remove(boxQuantity.Length - 3), CultureInfo.InvariantCulture);

        #endregion
    }
}
