namespace MyEntityMembershipService.Dtos
{
	public class PaginationToReturnDto<T> where T : class
	{
		public int Count { get; set; }

		public int TotalPages { get; set; }

		public int PageSize { get; set; }

		public int PageNumber { get; set; }

		public IReadOnlyList<T> Data { get; set; }

		public PaginationToReturnDto(int count, int totalPages, int pageSize, int pageNumber, IReadOnlyList<T> data)
		{

			Count = count;
			TotalPages = totalPages;
			PageSize = pageSize;
			PageNumber = pageNumber;
			Data = data;
		}
	}
}
