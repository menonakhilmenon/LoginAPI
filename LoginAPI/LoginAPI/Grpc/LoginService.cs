using Grpc.Core;
using LoginAPI.Helpers;
using LoginAPI.Models;
using LoginAPI.Protos;
using SessionKeyManager;
using System;
using System.Threading.Tasks;

namespace LoginAPI.Grpc
{
    public class LoginService : Protos.LoginService.LoginServiceBase
    {
        private readonly IDataAccess dataAccess;
        private readonly Random random;
        public LoginService(IDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
            random = new Random(DateTime.Now.Millisecond);
        }

        public override async Task<LoginReply> LoginUser(LoginRequest request, ServerCallContext context)
        {
            try
            {
                var op = await dataAccess.GetUserByEmail(request.Email);
                if (op != null && op.password == request.Password)
                {
                    return new LoginReply {  Activated = op.activated, Name = op.userName, ErrorCode = ErrorCode.Success };
                }
                else
                {
                    return new LoginReply { ErrorCode = ErrorCode.Fail };
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                return new LoginReply { ErrorCode = ErrorCode.FailUnknown };
            }
        }
        public override async Task<ActivationReply> ActivateUser(ActivationRequest request, ServerCallContext context)
        {
            try 
            {
                var op = await dataAccess.GetUserByEmail(request.Email);

                if (op!=null && op.otp == request.ActivationKey) 
                {
                    if (await dataAccess.SetUserActivation(request.Email, true)) 
                    {
                        return new ActivationReply { ErrorCode = ErrorCode.Success };
                    }
                }
                return new ActivationReply { ErrorCode = ErrorCode.Fail };
            }
            catch (Exception e) 
            {
                Console.WriteLine(e.ToString());
                return new ActivationReply { ErrorCode = ErrorCode.FailUnknown };
            }
        }
        public override async Task<CreateUserReply> CreateUser(CreateUserRequest request, ServerCallContext context)
        {
            try 
            {
                if((await dataAccess.GetUserByEmail(request.Email)) == null) 
                {
                    var bytes = new byte[8];
                    random.NextBytes(bytes);
                    var token = Convert.ToBase64String(bytes);
                    var user = new ClientInfo
                    {
                        activated = false,
                        email = request.Email,
                        userID = Guid.NewGuid().ToString(),
                        password = request.Password,
                        userName = request.Name,
                        otp = token
                    };
                    if(await dataAccess.CreateUser(user)) 
                    {
                        return new CreateUserReply { ErrorCode = ErrorCode.Success };
                    }
                    else 
                    {
                        return new CreateUserReply { ErrorCode = ErrorCode.Fail };
                    }
                }
                return new CreateUserReply { ErrorCode = ErrorCode.FailDuplicateEmail };
            }
            catch(Exception e) 
            {
                Console.WriteLine(e.ToString());
                return new CreateUserReply { ErrorCode = ErrorCode.FailUnknown };
            }
        }
    }
}
