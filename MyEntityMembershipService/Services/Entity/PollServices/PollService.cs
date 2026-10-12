using MyEntityMembershipService.DTOs.Entity.VoteDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.Entity.PollRepo;
using System.Net.Http.Headers;

namespace MyEntityMembershipService.Services.Entity.PollServices
{
    public class PollService : IPollService
    {
        private readonly IPollRepository _pollRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public PollService(IPollRepository pollRepository, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _pollRepository = pollRepository;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }


        public async Task<Response<object>> VoteOnAPoll(VoteDto pollDto)
        {
            try
            {

                var userValidation = await ValidateOnUser(pollDto.UserId);

                if (userValidation.Length > 0)
                    return new ResponseHandler().BadRequest<object>(userValidation);


                var poll = await _pollRepository.GetPollWithRelatedDataByIdAsync(pollDto.PollId, pollDto.UserId);


                if (poll is null)
                    return new ResponseHandler().BadRequest<object>("Invalid pollId");

                var pollChoicesIds = new HashSet<int>(poll.Choices.Select(c => c.Id));

                var isExists = pollDto.ChoicesIds.All(pollChoicesIds.Contains);


                if (!isExists)
                    return new ResponseHandler().BadRequest<object>("Invalid choicesIds");

                //


                if (!poll.HasMultiSelect)
                {
                    if (pollDto.ChoicesIds.Count > 1)
                        return new ResponseHandler().BadRequest<object>("this type of poll must have one choice");

                    //for (int i = 0; i < poll.Choices.Count; i++)
                    //{


                    //    if (poll.Choices.ToList()[i].Id == pollDto.ChoicesIds[0])
                    //    {
                    //        if (poll.Choices.ToList()[i].UserChoices.Count > 0)
                    //        {
                    //            return new ResponseHandler().BadRequest<object>("you already voted this choice");
                    //        }
                    //        else
                    //        {
                    //            poll.Choices.ToList()[i].UserChoices.Add(new Data.Entities.UserChoice() { Choice = poll.Choices.ToList()[i], CreatedBy = pollDto.UserId });

                    //            poll.Choices.ToList()[i].Vote = poll.Choices.ToList()[i].Vote + 1;


                    //        }

                    //    }
                    //    else
                    //    {
                    //        if (poll.Choices.ToList()[i].UserChoices.Count > 0)
                    //        {
                    //            var userChoice = poll.Choices.ToList()[i].UserChoices.FirstOrDefault(uc => uc.CreatedBy == pollDto.UserId);

                    //            poll.Choices.ToList()[i].UserChoices.Remove(userChoice);

                    //            poll.Choices.ToList()[i].Vote = poll.Choices.ToList()[i].Vote - 1;
                    //        }
                    //    }
                    //}

                    for (int i = 0; i < poll.Choices.Count; i++)
                    {
                        var choice = poll.Choices.ElementAt(i);

                        if (choice.Id == pollDto.ChoicesIds[0])
                        {
                            if (choice.UserChoices.Count > 0)
                            {
                                return new ResponseHandler().BadRequest<object>("you already voted this choice");
                            }
                            else
                            {
                                choice.UserChoices.Add(new UserChoice() { Choice = choice, CreatedBy = pollDto.UserId });

                                choice.Vote += 1;
                            }
                        }
                        else
                        {
                            var userChoice = choice.UserChoices.FirstOrDefault(uc => uc.CreatedBy == pollDto.UserId);

                            if (userChoice != null)
                            {
                                choice.UserChoices.Remove(userChoice);

                                choice.Vote -= 1;
                            }
                        }
                    }



                }
                else
                {



                    for (int i = 0; i < poll.Choices.Count; i++)
                    {
                        var choice = poll.Choices.ElementAt(i);

                        if (pollDto.ChoicesIds.Contains(choice.Id))
                        {
                            if (choice.UserChoices.Count > 0)
                            {
                                var userChoice = choice.UserChoices.FirstOrDefault(uc => uc.CreatedBy == pollDto.UserId);

                                if (userChoice != null)
                                {
                                    choice.UserChoices.Remove(userChoice);

                                    choice.Vote -= 1;
                                }
                            }
                            else
                            {
                                choice.UserChoices.Add(new UserChoice() { Choice = choice, CreatedBy = pollDto.UserId });

                                choice.Vote += 1;
                            }
                        }
                        continue;

                    }

                }

                await _pollRepository.SaveAsync();

                var result = new
                {
                    poll.PostId,
                    PollId = poll.Id,
                    HasMultiselect = poll.HasMultiSelect,

                    Choices = poll.Choices.Select(c => new
                    {
                        c.Id,
                        c.Text,
                        IsVoted = c.UserChoices.Any(c => c.CreatedBy == pollDto.UserId),
                        Votes = poll.Choices.Sum(choice => choice.Vote) > 0 ? (int)Math.Round((double)c.Vote / poll.Choices.Sum(choice => choice.Vote) * 100) : 0
                    }).ToList()
                };

                return new ResponseHandler().Success<object>(result);


            }
            catch (Exception ex)
            {

                return new ResponseHandler().BadRequest<object>("Something went wrong");
            }
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
