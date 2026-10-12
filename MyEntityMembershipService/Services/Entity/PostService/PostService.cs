using AutoMapper;
using MyEntityMembershipService.Dtos;
using MyEntityMembershipService.Helper;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using MyEntityMembershipService.Extensions;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using MyEntityMembershipService.Repositories.Entity.PostRepo;
using MyEntityMembershipService.DTOs.Entity.PostDtos;
using MyEntityMembershipService.Data.Entity.Entities.Enums;

using MyEntityMembershipService.Data;

namespace MyEntityMembershipService.Services.Entity.PostService
{
    public class PostService : IPostService
    {
        private readonly IPostRepo _postRepo;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public PostService(IPostRepo postRepo, IMapper mapper, IUnitOfWork unitOfWork, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _postRepo = postRepo;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        //public async Task<Response<PostToReturnDto>> CreatePost(PostDto postDto)
        //{
        //    try
        //    {
        //        DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);

        //        if (postDto.PublishDate < currentDate)
        //            return new ResponseHandler().BadRequest<PostToReturnDto>("invalid publishDate");

        //        var tenant = await _unitOfWork.Repository<Tenant>().GetByIdAsync(postDto.TenantId);

        //        if (tenant is null)
        //            return new ResponseHandler().NotFound<PostToReturnDto>("Inavlid tenantId");

        //        var httpClient = new HttpClient();

        //        var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();

        //        token = token.Replace("Bearer ", "");

        //        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //        var link = $"{_configuration["MySelfOldUrl:GetUsers"]}";

        //        var userAsList = new List<string>() { postDto.UserId };

        //        var userData = await httpClient.PostAsJson(link, userAsList);

        //        if (userData[0].id == null)
        //            return new ResponseHandler().NotFound<PostToReturnDto>("Invalid userId");


        //        var picUrl = string.Empty;

        //        if (postDto.PostPicture is not null)
        //        {
        //            picUrl = await DocumentSetting.AddToContainerAndGetUrl(_configuration["StorageConnectionString"], "buildupimagedocs", postDto.PostPicture);
        //        }

        //        var mappedPost = _mapper.Map<PostDto, Post>(postDto);

        //        mappedPost.PostPictureUrl = picUrl.IsNullOrEmpty() ? null : picUrl;

        //        mappedPost.CreatedBy = postDto.UserId;

        //        await _postRepo.Add(mappedPost);

        //        await _postRepo.SaveAsync();

        //        var mappedReturnedPost = _mapper.Map<Post, PostToReturnDto>(mappedPost);

        //        return new ResponseHandler().Created<PostToReturnDto>(mappedReturnedPost);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResponseHandler().BadRequest<PostToReturnDto>("Something went wrong");

        //    }

        //}

        //public async Task<Response<PostWithCommentsToReturnDto>> GetPostWithComments(int postId, string userId)
        //{
        //    try
        //    {
        //        var currentDate = DateOnly.FromDateTime(DateTime.Now);

        //        var post = await _postRepo.GetPostWithSavedBookMarkByIdAsync(postId, userId, currentDate);

        //        if (post is null)
        //            return new ResponseHandler().NotFound<PostWithCommentsToReturnDto>("Invalid postId");



        //        var mappedPost = _mapper.Map<Post, PostWithCommentsToReturnDto>(post);



        //        // to check on the userId along with postsOwner in one request

        //        var users = new List<string> { post.CreatedBy, userId };

        //        var httpClient = new HttpClient();

        //        var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();

        //        token = token.Replace("Bearer ", "");

        //        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //        var link = $"{_configuration["MySelfOldUrl:GetUsers"]}";

        //        var userData = await httpClient.PostAsJson(link, users);

        //        var userDataAsList = ((IEnumerable<dynamic>)userData).ToList();

        //        var isNotValid = userDataAsList.Exists(p => p.id == null);

        //        if (isNotValid)
        //            return new ResponseHandler().NotFound<PostWithCommentsToReturnDto>("Invalid userId");




        //        var postOwner = userDataAsList.FirstOrDefault(u => (string)u.id == post.CreatedBy);

        //        if (postOwner is not null)
        //        {
        //            mappedPost.PostedBy.Email = (string)postOwner.email;
        //            mappedPost.PostedBy.UserName = (string)postOwner.userName;
        //        }


        //        mappedPost.IsBookMarked = post.BookMarks.Any();

        //        var totalNumberOfComments = await _commentService.GetCountOfCommentsOfPost(mappedPost.Id);

        //        mappedPost.TotalNumberOfComments = totalNumberOfComments;

        //        var comments = await _commentService.GetCommentsWithReplies(mappedPost.Id);

        //        if (!comments.Succeeded)
        //            return new ResponseHandler().BadRequest<PostWithCommentsToReturnDto>(comments.Message);

        //        mappedPost.Comments = comments.Data;

        //        return new ResponseHandler().Success<PostWithCommentsToReturnDto>(mappedPost);

        //    }
        //    catch (Exception ex)
        //    {

        //        return new ResponseHandler().BadRequest<PostWithCommentsToReturnDto>("Something went wrong");
        //    }
        //}

        //public async Task<Response<PaginationToReturnDto<PostToReturnDto>>> GetPostsByTenantIdWithPagination(int tenantId, int pageNumber = 1, int pageSize = 10)
        //{

        //    try
        //    {
        //        var tenant = await _unitOfWork.Repository<Tenant>().GetByIdAsync(tenantId);

        //        if (tenant is null)
        //            return new ResponseHandler().NotFound<PaginationToReturnDto<PostToReturnDto>>("Inavlid tenantId");

        //        if (pageSize > 10 || pageSize < 1)
        //            pageSize = 10;

        //        if (pageNumber < 1)
        //            pageNumber = 1;


        //        var countOfPosts = await _postRepo.GetCountOfPostsByTenantIdAsync(tenantId);

        //        int totalPages = (int)Math.Ceiling(countOfPosts / (double)pageSize);

        //        var posts = await _postRepo.GetPostsByTenantIdWithPaginationAsync(tenantId, pageNumber, pageSize);

        //        var mappedPosts = _mapper.Map<IReadOnlyList<Post>, IReadOnlyList<PostToReturnDto>>(posts);

        //        var result = new PaginationToReturnDto<PostToReturnDto>(countOfPosts, totalPages, pageSize, pageNumber, mappedPosts);

        //        return new ResponseHandler().Success<PaginationToReturnDto<PostToReturnDto>>(result);
        //    }
        //    catch (Exception ex)
        //    {

        //        return new ResponseHandler().BadRequest<PaginationToReturnDto<PostToReturnDto>>("Something went wrong");
        //    }


        //}

        //public async Task<Response<PaginationToReturnDto<MobilePostToReturnDto>>> GetPostsForMobileByTenantIdWithPagination(GetAllPostsDto dto)
        //{

        //    try
        //    {
        //        var tenant = await _unitOfWork.Repository<Tenant>().GetByIdAsync(dto.TenantId);

        //        if (tenant is null)
        //            return new ResponseHandler().NotFound<PaginationToReturnDto<MobilePostToReturnDto>>("Inavlid tenantId");

        //        if (dto.PageSize > 10 || dto.PageSize < 1)
        //            dto.PageSize = 10;

        //        if (dto.PageNumber < 1)
        //            dto.PageNumber = 1;

        //        var currentDate = DateOnly.FromDateTime(DateTime.Now);

        //        var countOfPosts = await _postRepo.GetCountOfPostsByTenantIdAsync(dto.TenantId, currentDate);

        //        int totalPages = (int)Math.Ceiling(countOfPosts / (double)dto.PageSize);

        //        var postData = new SavedPostsOfUserDto { Date = currentDate, UserId = dto.UserId, TenantId = dto.TenantId, PageNumber = dto.PageNumber, PageSize = dto.PageSize };

        //        var posts = await _postRepo.GetPostsByTenantIdAndUserIdPaginationAsync(postData);

        //        var mappedPosts = _mapper.Map<IReadOnlyList<Post>, IReadOnlyList<MobilePostToReturnDto>>(posts);

        //        var users = posts.Where(p => p.CreatedBy != null).Select(p => p.CreatedBy).DistinctBy(c => c).ToList();

        //        // to check on ther userId along with postsOwner in one request
        //        users.Add(dto.UserId);


        //        var httpClient = new HttpClient();

        //        var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();

        //        token = token.Replace("Bearer ", "");

        //        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //        var link = $"{_configuration["MySelfOldUrl:GetUsers"]}";

        //        var userData = await httpClient.PostAsJson(link, users);

        //        var userDataAsList = ((IEnumerable<dynamic>)userData).ToList();

        //        var isNotValid = userDataAsList.Exists(p => p.id == null);

        //        if (isNotValid)
        //            return new ResponseHandler().NotFound<PaginationToReturnDto<MobilePostToReturnDto>>("Invalid userId");


        //        for (int i = 0; i < mappedPosts.Count; i++)
        //        {
        //            var postOwner = userDataAsList.FirstOrDefault(u => (string)u.id == mappedPosts[i].PostedBy.Id);

        //            if (postOwner is not null)
        //            {
        //                mappedPosts[i].PostedBy.Email = (string)postOwner.email;
        //                mappedPosts[i].PostedBy.UserName = (string)postOwner.userName;
        //            }

        //            var post = posts.FirstOrDefault(p => p.Id == mappedPosts[i].Id);

        //            if (post is not null)
        //                mappedPosts[i].IsBookMarked = post.BookMarks.Any();

        //            var totalNumberOfComments = await _commentService.GetCountOfCommentsOfPost(mappedPosts[i].Id);

        //            mappedPosts[i].TotalNumberOfComments = totalNumberOfComments;

        //        }

        //        var result = new PaginationToReturnDto<MobilePostToReturnDto>(countOfPosts, totalPages, dto.PageSize, dto.PageNumber, mappedPosts);

        //        return new ResponseHandler().Success<PaginationToReturnDto<MobilePostToReturnDto>>(result);

        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResponseHandler().BadRequest<PaginationToReturnDto<MobilePostToReturnDto>>("Something went wrong");
        //    }


        //}



        //public async Task<Response<string>> BookMarkPost(int postId, string userId)
        //{
        //    try
        //    {

        //        var currentDate = DateOnly.FromDateTime(DateTime.Now);

        //        var post = await _postRepo.GetPostByIdAndDateAsync(postId, currentDate);

        //        if (post is null)
        //            return new ResponseHandler().NotFound<string>("Invalid postId");

        //        var httpClient = new HttpClient();

        //        var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();

        //        token = token.Replace("Bearer ", "");

        //        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //        var link = $"{_configuration["MySelfOldUrl:GetUsers"]}";

        //        var userAsList = new List<string>() { userId };

        //        var userData = await httpClient.PostAsJson(link, userAsList);

        //        if (userData[0].id == null)
        //            return new ResponseHandler().NotFound<string>("Invalid userId");


        //        var isBookMarked = await _bookMarkService.IsPostBookMarkedByUser(postId, userId);

        //        if (isBookMarked)
        //            return new ResponseHandler().BadRequest<string>("You already have BookMarked this post");


        //        var BookMarkedPreviously = await _bookMarkService.IsPostBookMarkedPreviouslyByUser(postId, userId);


        //        if (BookMarkedPreviously is not null)
        //            _unitOfWork.Repository<BookMark>().UnSoftDelete(BookMarkedPreviously);

        //        else
        //            await _unitOfWork.Repository<BookMark>().Add(new BookMark { PostId = postId, UserId = userId });



        //        await _unitOfWork.Save();

        //        return new ResponseHandler().Success<string>("BookMarked Successfully");
        //    }
        //    catch (Exception ex)
        //    {

        //        return new ResponseHandler().BadRequest<string>("Something went wrong");
        //    }
        //}

        //public async Task<Response<string>> UnBookMarkPost(int postId, string userId)
        //{
        //    try
        //    {


        //        var currentDate = DateOnly.FromDateTime(DateTime.Now);

        //        var post = await _postRepo.GetPostByIdAndDateAsync(postId, currentDate);

        //        if (post is null)
        //            return new ResponseHandler().NotFound<string>("Invalid postId");

        //        var httpClient = new HttpClient();

        //        var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();

        //        token = token.Replace("Bearer ", "");

        //        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //        var link = $"{_configuration["MySelfOldUrl:GetUsers"]}";

        //        var userAsList = new List<string>() { userId };

        //        var userData = await httpClient.PostAsJson(link, userAsList);

        //        if (userData[0].id == null)
        //            return new ResponseHandler().NotFound<string>("Invalid userId");


        //        var isBookMarked = await _bookMarkService.IsPostBookMarkedByUser(postId, userId);

        //        if (!isBookMarked)
        //            return new ResponseHandler().BadRequest<string>("This post is already unbookmarked");


        //        var bookMarkedPost = await _bookMarkService.GetBookMarkedPostOfUserAsync(userId, postId);

        //        _unitOfWork.Repository<BookMark>().SoftDelete(bookMarkedPost);

        //        await _unitOfWork.Save();

        //        return new ResponseHandler().Success<string>("UnBookMarked Successfully");
        //    }
        //    catch (Exception ex)
        //    {

        //        return new ResponseHandler().BadRequest<string>("Something went wrong");
        //    }
        //}

        public async Task<Response<AnnouncementToReturnDto>> CreateAnnouncementPost(AnnouncementDto dto)
        {
            try
            {

                var validationResult = await ValidateOnPostCreation(dto);

                if (validationResult.Length > 0)
                    return new ResponseHandler().BadRequest<AnnouncementToReturnDto>(validationResult);



                var picUrls = new List<string>();

                //string? picUrl = null;

                for (int i = 0; i < dto.PostPictures.Count; i++)
                {
                    picUrls.Add(await DocumentSetting.AddToContainerAndGetUrl(_configuration["StorageConnectionString"], "buildupimagedocs", dto.PostPictures[i]));
                }

                //if (picUrls.Count > 0)
                //    picUrl = JsonSerializer.Serialize(picUrls);


                var mappedPost = _mapper.Map<AnnouncementDto, Post>(dto);

                mappedPost.PostPictureUrl = dto.PostPictures.Count == 0 ? null : picUrls;

                if (dto.IsPinned && dto.IsDraft == false)
                {
                    var pinnedPosts = await _postRepo.GetPinnedPostsAsync(dto.TenantId);

                    //if (pinnedPosts.Count > 2)
                    //    pinnedPosts[2].IsPinned = false;

                    pinnedPosts = pinnedPosts.Select((pp, i) =>
                    {
                        if (i > 1)
                            pp.IsPinned = false;
                        return pp;
                    }).ToList();
                }

                await _postRepo.Add(mappedPost);

                await _postRepo.SaveAsync();

                var mappedPostToReturn = _mapper.Map<Post, AnnouncementToReturnDto>(mappedPost);

                mappedPostToReturn.PostPictures = picUrls;

                return new ResponseHandler().Created(mappedPostToReturn);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<AnnouncementToReturnDto>("Something went wrong");

            }

        }


        public async Task<Response<NewsFeedToReturnDto>> CreateNewsFeedPost(NewsFeedDto dto)
        {
            try
            {

                var validationResult = await ValidateOnPostCreation(dto);

                if (validationResult.Length > 0)
                    return new ResponseHandler().BadRequest<NewsFeedToReturnDto>(validationResult);


                var picUrls = new List<string>();

                //string? picUrl = null;

                for (int i = 0; i < dto.PostPictures.Count; i++)
                {
                    picUrls.Add(await DocumentSetting.AddToContainerAndGetUrl(_configuration["StorageConnectionString"], "buildupimagedocs", dto.PostPictures[i]));
                }

                //if (picUrls.Count > 0)
                //    picUrl = JsonSerializer.Serialize(picUrls);


                var mappedPost = _mapper.Map<NewsFeedDto, Post>(dto);

                mappedPost.PostPictureUrl = dto.PostPictures.Count == 0 ? null : picUrls;

                var distinctTags = dto.PostTags.DistinctBy(pt => pt).ToList();

                var tags = new List<PostTag>();

                var existingTags = new List<PostTag>();

                for (int i = 0; i < distinctTags.Count; i++)
                {
                    Expression<Func<PostTag, bool>> criteria = (pt) => pt.Tag.ToLower().Replace(" ", "") == distinctTags[i].ToLower().Replace(" ", "");

                    var postTag = await _unitOfWork.Repository<PostTag>().FindByCriteriaAsync(criteria);

                    if (postTag is not null)
                        existingTags.Add(postTag);
                    else
                        tags.Add(new PostTag { Tag = distinctTags[i] });
                }

                mappedPost.PostTags = tags;

                await _postRepo.Add(mappedPost);

                await _postRepo.SaveAsync();

                for (int i = 0; i < existingTags.Count; i++)
                {
                    mappedPost.PostTags.Add(existingTags[i]);
                }
                await _postRepo.SaveAsync();

                var mappedPostToReturn = _mapper.Map<Post, NewsFeedToReturnDto>(mappedPost);

                mappedPostToReturn.PostPictures = picUrls;

                mappedPostToReturn.PostTags = mappedPost.PostTags.Select(pt => (object)new { pt.Id, pt.Tag }).ToList();

                return new ResponseHandler().Created(mappedPostToReturn);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<NewsFeedToReturnDto>("Something went wrong");

            }

        }

        public async Task<Response<PollToReturnDto>> CreatePollPost(PollDto dto)
        {
            try
            {

                var validationResult = await ValidateOnPollCreation(dto);

                if (validationResult.Length > 0)
                    return new ResponseHandler().BadRequest<PollToReturnDto>(validationResult);


                var mappedPost = _mapper.Map<PollDto, Post>(dto);


                var choices = new List<Choice>();

                for (int i = 0; i < dto.Choices.Count; i++)
                {
                    choices.Add(new Choice { Text = dto.Choices[i] });
                }

                var poll = new Poll() { HasMultiSelect = dto.HasMultiSelect, Question = dto.Question, Choices = choices };

                mappedPost.Poll = poll;

                await _postRepo.Add(mappedPost);

                await _postRepo.SaveAsync();

                var mappedPostToReturn = _mapper.Map<Post, PollToReturnDto>(mappedPost);

                mappedPostToReturn.Question = dto.Question;

                mappedPostToReturn.Choices = dto.Choices;

                mappedPostToReturn.HasMultiSelect = dto.HasMultiSelect;



                return new ResponseHandler().Created(mappedPostToReturn);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<PollToReturnDto>("Something went wrong");

            }

        }

        public async Task<Response<HightlightToReturnDto>> CreateHighlightPost(HighlightDto dto)
        {
            try
            {

                var validationResult = await ValidateOnPostCreation(dto);

                if (validationResult.Length > 0)
                    return new ResponseHandler().BadRequest<HightlightToReturnDto>(validationResult);


                var picUrls = new List<string>();

                //string? picUrl = null;

                for (int i = 0; i < dto.PostPictures.Count; i++)
                {
                    picUrls.Add(await DocumentSetting.AddToContainerAndGetUrl(_configuration["StorageConnectionString"], "buildupimagedocs", dto.PostPictures[i]));
                }

                //if (picUrls.Count > 0)
                //    picUrl = JsonSerializer.Serialize(picUrls);


                var mappedPost = _mapper.Map<HighlightDto, Post>(dto);

                mappedPost.PostPictureUrl = dto.PostPictures.Count == 0 ? null : picUrls;



                await _postRepo.Add(mappedPost);

                await _postRepo.SaveAsync();

                var mappedPostToReturn = _mapper.Map<Post, HightlightToReturnDto>(mappedPost);

                mappedPostToReturn.PostPictures = picUrls;


                return new ResponseHandler().Created(mappedPostToReturn);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<HightlightToReturnDto>("Something went wrong");

            }

        }

        public async Task<Response<PaginationToReturnDto<object>>> GetAllPosts(GetAllPostsWebDto dto)
        {
            try
            {
                if (dto.PageSize > 10 || dto.PageSize < 1)
                    dto.PageSize = 10;

                if (dto.PageNumber < 1)
                    dto.PageNumber = 1;

                Expression<Func<Post, bool>> criteria = (p) => p.TenantId == dto.TenantId && p.IsDraft == false && (string.IsNullOrEmpty(dto.SearchTerm) || p.PostTitle.ToLower().Replace(" ", "").Contains(dto.SearchTerm) || p.Poll.Question.ToLower().Replace(" ", "").Contains(dto.SearchTerm));

                var countOfPosts = await _postRepo.GetCountOfPostsByTenantIdAsync(criteria);

                int totalPages = (int)Math.Ceiling(countOfPosts / (double)dto.PageSize);

                var posts = await _postRepo.GetAllPosts(dto);

                //var dynamicPosts = ((IEnumerable<dynamic>)posts).ToList();

                //for (int i = 0; i < dynamicPosts.Count; i++)
                // {
                //     if (dynamicPosts[i]?.PostPictures.Length > 0)

                //         dynamicPosts[i].PostPictures=JsonSerializer.Deserialize<List<string>>
                // }

                var paginationToReturnDto = new PaginationToReturnDto<object>(countOfPosts, totalPages, dto.PageSize, dto.PageNumber, posts);

                return new ResponseHandler().Success(paginationToReturnDto);

                //var dynamicPosts = ((IEnumerable<dynamic>)posts).ToList();



                //if (dynamicPosts.Exists(p => (PostType)p.PostType == PostType.Announcement && (bool)p.IsPinned == true))
                //{
                //    dynamicPosts = dynamicPosts.OrderByDescending(p => (p.GetType().GetProperty("IsPinned") != null && (bool)p.IsPinned == true)).ToList();

                //    var paginatedDynamicPosts = dynamicPosts.Skip(dto.PageSize * (dto.PageNumber - 1)).Take(dto.PageSize).ToList();

                //    var paginationToReturnDto = new PaginationToReturnDto<object>(countOfPosts, totalPages, dto.PageSize, dto.PageNumber, paginatedDynamicPosts);

                //    return new ResponseHandler().Success<PaginationToReturnDto<object>>(paginationToReturnDto);
                //}

                //var paginatedPosts = posts.Skip(dto.PageSize * (dto.PageNumber - 1)).Take(dto.PageSize).ToList();

                //var paginationToReturn = new PaginationToReturnDto<object>(countOfPosts, totalPages, dto.PageSize, dto.PageNumber, paginatedPosts);

                //return new ResponseHandler().Success<PaginationToReturnDto<object>>(paginationToReturn);




            }
            catch (Exception ex)
            {

                return new ResponseHandler().BadRequest<PaginationToReturnDto<object>>("Something went wrong");
            }
        }

        public async Task<Response<PaginationToReturnDto<object>>> GetAllMobilePosts(GetAllMobilePostsDto dto)
        {
            try
            {
                if (dto.PageSize > 10 || dto.PageSize < 1)
                    dto.PageSize = 10;

                if (dto.PageNumber < 1)
                    dto.PageNumber = 1;
                var CurrentDate = DateOnly.FromDateTime(DateTime.Now);

                if (!string.IsNullOrEmpty(dto.SearchTerm))
                    dto.SearchTerm = dto.SearchTerm.ToLower().Replace(" ", "");

                Expression<Func<Post, bool>> criteria = (p) => p.TenantId == dto.TenantId && p.IsDraft == false && (string.IsNullOrEmpty(dto.SearchTerm) || p.PostTitle.ToLower().Replace(" ", "").Contains(dto.SearchTerm) || p.Poll.Question.ToLower().Replace(" ", "").Contains(dto.SearchTerm));

                var countOfPosts = await _postRepo.GetCountOfPostsByTenantIdAsync(criteria);

                int totalPages = (int)Math.Ceiling(countOfPosts / (double)dto.PageSize);

                var posts = await _postRepo.GetAllMobilePosts(dto, CurrentDate);

                //var dynamicPosts = ((IEnumerable<dynamic>)posts).ToList();

                //for (int i = 0; i < dynamicPosts.Count; i++)
                // {
                //     if (dynamicPosts[i]?.PostPictures.Length > 0)

                //         dynamicPosts[i].PostPictures=JsonSerializer.Deserialize<List<string>>
                // }

                var paginationToReturnDto = new PaginationToReturnDto<object>(countOfPosts, totalPages, dto.PageSize, dto.PageNumber, posts);

                return new ResponseHandler().Success(paginationToReturnDto);

                //var dynamicPosts = ((IEnumerable<dynamic>)posts).ToList();



                //if (dynamicPosts.Exists(p => (PostType)p.PostType == PostType.Announcement && (bool)p.IsPinned == true))
                //{
                //    dynamicPosts = dynamicPosts.OrderByDescending(p => (p.GetType().GetProperty("IsPinned") != null && (bool)p.IsPinned == true)).ToList();

                //    var paginatedDynamicPosts = dynamicPosts.Skip(dto.PageSize * (dto.PageNumber - 1)).Take(dto.PageSize).ToList();

                //    var paginationToReturnDto = new PaginationToReturnDto<object>(countOfPosts, totalPages, dto.PageSize, dto.PageNumber, paginatedDynamicPosts);

                //    return new ResponseHandler().Success<PaginationToReturnDto<object>>(paginationToReturnDto);
                //}

                //var paginatedPosts = posts.Skip(dto.PageSize * (dto.PageNumber - 1)).Take(dto.PageSize).ToList();

                //var paginationToReturn = new PaginationToReturnDto<object>(countOfPosts, totalPages, dto.PageSize, dto.PageNumber, paginatedPosts);

                //return new ResponseHandler().Success<PaginationToReturnDto<object>>(paginationToReturn);




            }
            catch (Exception ex)
            {

                return new ResponseHandler().BadRequest<PaginationToReturnDto<object>>("Something went wrong");
            }
        }

        public async Task<Response<int>> LikeUnLikePost(int postId, string userId)
        {
            try
            {

                var post = await _postRepo.GetPostWithLikesAsync(postId, userId);


                var postTypeValidation = ValidateOnCommentsForPostType(post);

                if (postTypeValidation.Length > 0)
                    return new ResponseHandler().BadRequest<int>(postTypeValidation);

                var userValidation = await ValidateOnUser(userId);

                if (userValidation.Length > 0)
                    return new ResponseHandler().BadRequest<int>(userValidation);


                if (post.Likes.Count == 0)
                {
                    post.Likes.Add(new Like { CreatedBy = userId, PostId = post.Id });

                    await _postRepo.SaveAsync();

                    var count = await _unitOfWork.Repository<Like>().GetCountByCriteriaAsync(l => l.PostId == postId);

                    return new ResponseHandler().Created(count);
                }
                else
                {
                    _unitOfWork.Repository<Like>().Delete(post.Likes.ToList()[0]);

                    await _postRepo.SaveAsync();

                    var count = await _unitOfWork.Repository<Like>().GetCountByCriteriaAsync(l => l.PostId == postId);

                    return new ResponseHandler().Success(count);

                }



            }
            catch (Exception ex)
            {

                return new ResponseHandler().BadRequest<int>("Something went wrong");
            }
        }

        public async Task<Response<int>> SharePost(int postId, string userId)
        {
            try
            {
                var post = await _postRepo.GetPostByIdAsyncAndIsNotDraft(postId);


                var postTypeValidation = ValidateOnLikesForPostType(post);

                if (postTypeValidation.Length > 0)
                    return new ResponseHandler().BadRequest<int>(postTypeValidation);

                var userValidation = await ValidateOnUser(userId);

                if (userValidation.Length > 0)
                    return new ResponseHandler().BadRequest<int>(userValidation);

                await _unitOfWork.Repository<Share>().Add(new Share { CreatedBy = userId, PostId = post.Id });

                await _postRepo.SaveAsync();

                var count = await _unitOfWork.Repository<Share>().GetCountByCriteriaAsync(l => l.PostId == postId);



                return new ResponseHandler().Created(count);

            }
            catch (Exception ex)
            {

                return new ResponseHandler().BadRequest<int>("Something went wrong");
            }
        }

        public async Task<Post> GetPostByIdAsyncAndIsNotDraft(int postId)
            => await _postRepo.GetPostByIdAsyncAndIsNotDraft(postId);

        public async Task<Response<object>> GetPostByIdAsync(int postId)
        {
            try
            {
                var post = await _postRepo.GetPostByIdAsyncAndIsNotDraft(postId);

                if (post is null)
                    return new ResponseHandler().NotFound<object>("Invalid postId");


                if (post.PostType == PostType.Announcement)
                {
                    var postToReturn = await _postRepo.GetAnnouncementPostByIdAsync(postId);

                    return new ResponseHandler().Success(postToReturn);
                }
                else if (post.PostType == PostType.NewsFeed)
                {
                    var postToReturn = await _postRepo.GetNewsFeedPostByIdAsync(postId);

                    return new ResponseHandler().Success(postToReturn);
                }
                else if (post.PostType == PostType.EventPoll)
                {
                    var postToReturn = await _postRepo.GetEventPollByIdAsync(postId);

                    return new ResponseHandler().Success(postToReturn);
                }
                else
                {
                    var postToReturn = await _postRepo.GetHighLightsByIdAsync(postId);

                    return new ResponseHandler().Success(postToReturn);
                }


            }
            catch (Exception ex)
            {

                return new ResponseHandler().BadRequest<object>("Something went wrong");
            }
        }

        public async Task<Response<IReadOnlyList<object>>> GetPostsOfOtherEntities(int tenantId)
        {
            try
            {



                var posts = await _postRepo.GetPostsOfOtherEntitiesAsync(tenantId);

                return new ResponseHandler().Success(posts);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<IReadOnlyList<object>>("Something went wrong");

            }
        }

        private async Task<string> ValidateOnPostCreation(IPostDto dto)
        {


            if (dto.PublishDate.HasValue)
            {
                DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);

                if (dto.PublishDate.Value < currentDate.AddDays(1))
                    return "publish date must be after current date by day atleast";
            }
            var tenant = await _unitOfWork.Repository<Tenant>().GetByIdAsync(dto.TenantId);

            if (tenant is null)
                return "Inavlid tenantId";

            var httpClient = new HttpClient();

            var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();

            token = token.Replace("Bearer ", "");

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var link = $"{_configuration["ApiSettings:Gateway"]}{_configuration["MySelfOldUrl:GetUsers"]}";

            var userAsList = new List<string>() { dto.CreatedBy };

            var userData = await httpClient.PostAsJson(link, userAsList);

            if (userData[0].id == null)
                return "Invalid userId";

            var pics = new List<IFormFile>();

            for (int i = 0; i < dto.PostPictures.Count; i++)
            {
                if (!DocumentSetting.IsFileValidFormatAndDoesnotExceedMaxSize(dto.PostPictures[i]))
                    return "Invalid format or exceeded max file size";

                if (i == 0)
                {
                    pics.Add(dto.PostPictures[i]);
                    continue;
                }


                bool isExists = pics.Exists(p => p.FileName == dto.PostPictures[i].FileName);

                if (isExists)
                    return "Image name must be unique";

            }

            return "";
        }

        private async Task<string> ValidateOnPollCreation(PollDto dto)
        {
            if (dto.PublishDate.HasValue)
            {
                DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);

                if (dto.PublishDate.Value < currentDate.AddDays(1))
                    return "publish date must be after current date by day atleast";
            }

            var tenant = await _unitOfWork.Repository<Tenant>().GetByIdAsync(dto.TenantId);

            if (tenant is null)
                return "Inavlid tenantId";

            var httpClient = new HttpClient();

            var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();

            token = token.Replace("Bearer ", "");

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var link = $"{_configuration["ApiSettings:Gateway"]}{_configuration["MySelfOldUrl:GetUsers"]}";

            var userAsList = new List<string>() { dto.CreatedBy };

            var userData = await httpClient.PostAsJson(link, userAsList);

            if (userData[0].id == null)
                return "Invalid userId";

            dto.Choices = dto.Choices.DistinctBy(c => c).ToList();

            if (dto.Choices.Count < 2)
                return "poll choices must be 2 atleast and unique";

            return "";
        }

        private string ValidateOnCommentsForPostType(Post post)
        {

            if (post is null)
                return "Invalid postId";

            if (post.PostType == PostType.Announcement)
                return "Announcements don't have comments";

            if (post.PostType == PostType.EventPoll)
                return "Event Polls don't have comments";

            if (post.PostType == PostType.EventUpdate)
                return "Event Updates don't have comments";

            if (post.PostType == PostType.NewsFeed && post.HasComments == false)
                return "comments are disabled for this post";

            if (post.PostType == PostType.HighLights && post.HasComments == false)
                return "comments are disabled for this post";

            return "";
        }

        private string ValidateOnLikesForPostType(Post post)
        {

            if (post is null)
                return "Invalid postId";

            if (post.PostType == PostType.Announcement)
                return "Announcements don't have likes";

            if (post.PostType == PostType.EventPoll)
                return "Event Polls don't have likes";

            if (post.PostType == PostType.EventUpdate)
                return "Event Updates don't have likes";

            if (post.PostType == PostType.NewsFeed && post.HasComments == false)
                return "likes are disabled for this post";

            if (post.PostType == PostType.HighLights && post.HasComments == false)
                return "likes are disabled for this post";

            return "";
        }

        private async Task<string> ValidateOnUser(string userId)
        {
            var httpClient = new HttpClient();

            var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();

            token = token.Replace("Bearer ", "");

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var link = $"{_configuration["ApiSettings:Gateway"]}{_configuration["MySelfOldUrl:GetUsers"]}";

            var userAsList = new List<string>() { userId };

            var userData = await httpClient.PostAsJson(link, userAsList);

            if (userData[0].id == null)
                return "Invalid userId";

            return "";

        }



    }
}
