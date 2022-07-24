using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ApplicationCore.Helpers;
using Infrastructure.DataAccess;

namespace ApplicationCore.Services
{
	public interface IManualsService
	{
		Task<IEnumerable<Manual>> FetchAllAsync();
		Task<IEnumerable<Manual>> FetchAsync(bool active = true);
		Task<IEnumerable<Manual>> FetchAsync(int parentId, bool features = false);
		Task<Manual> GetByIdAsync(int id, bool subItems = false);
		Task<Manual> CreateAsync(Manual manual);
		Task UpdateAsync(Manual manual);
		Task UpdateAsync(Manual existingEntity, Manual model);
		Task RemoveAsync(Manual manual);

		Feature GetFeatureById(int id);
		Task<Feature> GetFeatureByIdAsync(int id);
		Task<Feature> CreateFeatureAsync(Feature feature);
		Task UpdateFeatureAsync(Feature existingEntity, Feature feature);
		Task RemoveFeatureAsync(Feature feature);
	}

	public class ManualsService : BaseCategoriesService<Manual>, IManualsService
	{
		private readonly IDefaultRepository<Manual> _manualRepository;
		private readonly IDefaultRepository<Feature> _featureRepository;

		public ManualsService(IDefaultRepository<Manual> manualRepository, IDefaultRepository<Feature> featureRepository)
		{
			_manualRepository = manualRepository;
			_featureRepository = featureRepository;
		}

		public async Task<IEnumerable<Manual>> FetchAllAsync() => await _manualRepository.ListAsync(new ManualFilterSpecification());


		public async Task<IEnumerable<Manual>> FetchAsync(bool active = true)
		{
			int parentId = 0;
			bool features = true;
			var manuals = await FetchAsync(parentId, features);
			manuals = manuals.Where(x => x.Active == active).ToList();

			var allItems = await FetchAllAsync();
			foreach (var item in manuals)
			{
				item.LoadSubItems(allItems);
			} 

			return manuals;
		}

		public async Task<IEnumerable<Manual>> FetchAsync(int parentId, bool features = false)
			=> await _manualRepository.ListAsync(new ManualParentFilterSpecification(parentId, features));


		
		public async Task<Manual> GetByIdAsync(int id, bool subItems = false)
		{
			var spec = new ManualFilterSpecification(id);
			var manual = _manualRepository.GetSingleBySpec(spec);
			
			if (subItems)
			{
				var allItems = await FetchAllAsync();
				manual.LoadSubItems(allItems);
			}

			return manual;

		}

		public async Task<Manual> CreateAsync(Manual manual) => await _manualRepository.AddAsync(manual);

		public async Task UpdateAsync(Manual manual) => await _manualRepository.UpdateAsync(manual);

		public async Task UpdateAsync(Manual existingEntity, Manual model) => await _manualRepository.UpdateAsync(existingEntity, model);

		public async Task UpdateOrderAsync(int target, int replace, bool up)
		{
			var targetEntity = await _manualRepository.GetByIdAsync(target);
			int targetOrder = targetEntity.Order;

			var replaceEntity = await _manualRepository.GetByIdAsync(replace);
			int replaceOrder = replaceEntity.Order;

			targetEntity.Order = replaceOrder;
			replaceEntity.Order = targetOrder;

			if (targetEntity.Order == replaceEntity.Order)
			{
				if (up) replaceEntity.Order += 1;
				else targetEntity.Order += 1;
			}

			_manualRepository.UpdateRange(new List<Manual> { targetEntity, replaceEntity });
		}

		public async Task RemoveAsync(Manual manual)
		{
			manual.Removed = true;
			await _manualRepository.UpdateAsync(manual);
		}


		public Feature GetFeatureById(int id) => _featureRepository.GetSingleBySpec(new FeatureFilterSpecification(id));

		public async Task<Feature> GetFeatureByIdAsync(int id) => await _featureRepository.GetByIdAsync(id);

		public async Task<Feature> CreateFeatureAsync(Feature feature) => await _featureRepository.AddAsync(feature);

		public async Task UpdateFeatureAsync(Feature existingEntity, Feature feature) => await _featureRepository.UpdateAsync(existingEntity, feature);

		public async Task RemoveFeatureAsync(Feature feature)
		{
			feature.Removed = true;
			await _featureRepository.UpdateAsync(feature);
		}
	}
}
