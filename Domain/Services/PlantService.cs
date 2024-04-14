using AutoMapper;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Models;
using Domain.Models.Enums.Product;
using Domain.Objects.Requests.Customer;
using Domain.Objects.Requests.Plant;
using Domain.Objects.Responses.Asset;
using Domain.Objects.Responses.Machine;
using Domain.Utils.Helpers;

namespace Domain.Services
{
    public class PlantService(IPlantRepository plantRepository, IUserRepository userRepository, IAssetRepository assetRepository, IMapper mapper) : IPlantService
    {
        public async Task<IEnumerable<PlantToTableResponse>?> GetToTable(int currentPage, string? plantName, string? plantCnpj) => await plantRepository.GetPlantTypedData<PlantToTableResponse>(currentPage, plantName, plantCnpj);

        public async Task<PlantToFilterResponse?> GetToFilter(int currentPage, string? plantName, string? plantCnpj)
        {
            var userId = HttpContextHelper.GetUserId();
            var assetId = await userRepository.GetAssetIdById(userId);

            PlantToFilterResponse plantToFilterResponse = new()
            {
                SelectedAssetId = assetId,
                PlantData = await plantRepository.GetPlantTypedData<PlantToFilterData>(currentPage, plantName, plantCnpj, userId)
            };

            if (assetId == null)
                return plantToFilterResponse;

            plantToFilterResponse.SelectedPlantId = await assetRepository.GetPlantIdById(assetId.Value);
            plantToFilterResponse.AssetData = await assetRepository.GetAssetTypedData<AssetToFilterResponse>(currentPage, plantToFilterResponse.SelectedPlantId, null);

            return plantToFilterResponse;
        }

        public async Task Save(SavePlantRequest savePlantRequest)
        {
            if (!await userRepository.HasUserById(savePlantRequest.UserId))
                throw new InvalidOperationException("UserNotFound");

            await plantRepository.HasPlantWithTheSameInfo(savePlantRequest.Cnpj, savePlantRequest.Name);

            await plantRepository.Save(mapper.Map<Plant>(savePlantRequest));
        }

        public async Task Update(UpdatePlantRequest updatePlantRequest)
        {
            var plant = await plantRepository.GetById(updatePlantRequest.PlantId)
                    ?? throw new InvalidOperationException("PlantNotFound");

            await plantRepository.HasPlantWithTheSameInfo(updatePlantRequest.Cnpj, updatePlantRequest.Name, updatePlantRequest.PlantId);

            plant = updatePlantRequest.MapIgnoringNullProperties(plant);
            plant.UpdatedAt = DateTime.Now;

            await plantRepository.Update(plant);
        }

        public async Task Delete(int plantId) => await plantRepository.Delete(plantId);

        public async Task<PlantSchema> GetSchema(int plantId, int assetId)
        {
            var plantInfo = await plantRepository.GetSchemaById(plantId, assetId)
                ?? throw new InvalidOperationException("PlantNotFound");

            var assetSchema = plantInfo.AssetSchema
                ?? throw new InvalidOperationException("AssetNotFound");

            if (assetSchema.ProductSchema != null && assetSchema.ProductSchema.Count != 0)
                foreach (var product in assetSchema.ProductSchema)
                {
                    if (product.LayoutSchema != null && product.LayoutSchema.Count != 0)
                    {
                        var magnaPayload = GetProductSchema(EProductType.Magna);
                        var optoClassPayload = GetProductSchema(EProductType.OptoClass);

                        switch (product.ProductType)
                        {
                            case EProductType.Magna:
                                product.PayloadSchema = magnaPayload;
                                break;
                            case EProductType.OptoClass:
                                product.PayloadSchema = optoClassPayload;
                                break;
                        }
                    }
                }

            return plantInfo;
        }

        private static object GetProductSchema(EProductType productType)
        {
            var magnaPayload = new
            {
                MachineStatus = "",
                CurrentSpeed = "",
                ProgSpeed = "",
                WeightQuantity = new
                {
                    P1 = "",
                    P2 = "",
                    P3 = "",
                    P4 = "",
                    P5 = "",
                    P6 = "",
                    P7 = ""
                },
                ProductionQuantity = new
                {
                    P1 = "",
                    P2 = "",
                    P3 = "",
                    P4 = "",
                    P5 = "",
                    P6 = "",
                    P7 = ""
                },
                CrackedQuantity = new
                {
                    P1 = "",
                    P2 = "",
                    P3 = "",
                    P4 = "",
                    P5 = "",
                    P6 = "",
                    P7 = ""
                }
            };

            var optoClassPayload = new
            {
                DevStatus = "",
                Defects = new
                {
                    Dirty = "",
                    Cracked = "",
                    Broken = "",
                    Leaked = ""
                },
                Production = new
                {
                    Total = "",
                    Bad = "",
                    White = "",
                    WhiteBad = "",
                    Fill = "",
                    Flow = ""
                }
            };

            return productType == EProductType.Magna ? magnaPayload : optoClassPayload;
        }
    }
}
