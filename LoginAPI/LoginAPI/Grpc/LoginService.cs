using Grpc.Core;
using LoginAPI.Helpers;
using LoginAPI.Models;
using LoginAPI.Protos;
using SessionKeyManager;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace LoginAPI.Grpc
{
    public class LoginService : Protos.LoginService.LoginServiceBase
    {
        private readonly IDataAccess dataAccess;
        private readonly EmailHelper emailHelper;
        private readonly RNGCryptoServiceProvider random;
        public LoginService(IDataAccess dataAccess, EmailHelper emailHelper)
        {
            random = new RNGCryptoServiceProvider();
            this.dataAccess = dataAccess;
            this.emailHelper = emailHelper;
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
                    if (await dataAccess.ActivateUser(request.Email)) 
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
                    random.GetBytes(bytes);
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
                    if(await dataAccess.CreateUser(user) && await emailHelper.SendActivationMail(user.email,user.otp)) 
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
     
        public override async Task<ChangePasswordReply> ChangePassword(ChangePasswordRequest request, ServerCallContext context)
        {
            try
            {
                var res = await dataAccess.GetUserByEmail(request.Email);
                if (res != null && dataAccess.ComparePassword(request.Password, res.password))
                {
                    if (await dataAccess.SetPassword(res.userID, request.NewPassword))
                    {
                        return new ChangePasswordReply { ErrorCode = ErrorCode.Success };
                    }
                }
                return new ChangePasswordReply { ErrorCode = ErrorCode.Fail };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return new ChangePasswordReply { ErrorCode = ErrorCode.FailUnknown };
            }
        }
       
        public override async Task<ResetPasswordReply> ResetPassword(ResetPasswordRequest request, ServerCallContext context)
        {
            try
            {
                var res = await dataAccess.GetUserByEmail(request.Email);
                if (res != null && res.passwordOtp == request.Otp)
                {
                    if (await dataAccess.SetPassword(res.userID, request.NewPassword))
                    {
                        return new ResetPasswordReply { ErrorCode = ErrorCode.Success };
                    }
                }
                return new ResetPasswordReply { ErrorCode = ErrorCode.Fail };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return new ResetPasswordReply { ErrorCode = ErrorCode.FailUnknown };
            }
        }

        public override async Task<ResendOTPReply> ResendOTP(ResendOTPRequest request, ServerCallContext context)
        {
            try 
            {
                var user = await dataAccess.GetUserByEmail(request.Email);
                if (user != null)
                {
                    var bytes = new byte[8];
                    random.GetBytes(bytes);
                    var token = Convert.ToBase64String(bytes);

                    if (await dataAccess.ChangeOTP(user.userID,token) && await emailHelper.SendActivationMail(user.email, token))
                    {
                        return new ResendOTPReply { ErrorCode = ErrorCode.Success };
                    }
                }
                return new ResendOTPReply { ErrorCode = ErrorCode.Fail };
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                return new ResendOTPReply { ErrorCode = ErrorCode.FailUnknown };
            }
        }

        public override async Task<SendPasswordOTPReply> SendPasswordOTP(SendPasswordOTPRequest request, ServerCallContext context)
        {
            try
            {
                var user = await dataAccess.GetUserByEmail(request.Email);
                if (user != null)
                {
                    var bytes = new byte[8];
                    random.GetBytes(bytes);
                    var token = Convert.ToBase64String(bytes);

                    if (await dataAccess.ChangePasswordOTP(user.userID, token) && await emailHelper.SendForgetPasswordMail(user.email, token))
                    {
                        return new SendPasswordOTPReply { ErrorCode = ErrorCode.Success };
                    }
                }
                return new SendPasswordOTPReply { ErrorCode = ErrorCode.Fail };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return new SendPasswordOTPReply { ErrorCode = ErrorCode.FailUnknown };
            }
        }



    }
}
