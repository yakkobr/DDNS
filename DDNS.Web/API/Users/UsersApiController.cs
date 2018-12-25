﻿using DDNS.Entity.Users;
using DDNS.Provider.LoginLog;
using DDNS.Provider.Users;
using DDNS.Utility;
using DDNS.ViewModel.Response;
using DDNS.ViewModel.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDNS.Web.API.Users
{
    [Route("api")]
    [ApiController]
    [Authorize]
    public class UsersApiController : ControllerBase
    {
        private readonly UsersProvider _usersProvider;
        private readonly LoginLogProvider _loginLogProvider;

        public UsersApiController(UsersProvider usersProvider, LoginLogProvider loginLogProvider)
        {
            _usersProvider = usersProvider;
            _loginLogProvider = loginLogProvider;
        }

        /// <summary>
        /// 用户列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="userName"></param>
        /// <param name="email"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("users")]
        public async Task<ResponseViewModel<List<UsersViewModel>>> Users(int page, int limit, string userName, string email, int status)
        {
            var users = new List<UsersViewModel>();

            var list = await _usersProvider.UserList(userName, email, status);

            var curList = list.ToList().Skip(limit * (page - 1)).Take(limit).ToList();
            curList.ForEach(x =>
            {
                var vm = new UsersViewModel
                {
                    Id = x.Id,
                    UserName = x.UserName,
                    Email = x.Email,
                    Status = x.Status,
                    RegisterTime = x.RegisterTime.ToLongDateString(),
                };

                users.Add(vm);
            });

            var result = new ResponseViewModel<List<UsersViewModel>>
            {
                Count = list.ToList().Count,
                Data = users
            };

            return result;
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("add_user")]
        public async Task<ResponseViewModel<bool>> AddUser(EditUserViewModel vm)
        {
            var data = new ResponseViewModel<bool>();

            var user = new UsersEntity
            {
                UserName = vm.UserName,
                Email = vm.Email,
                Password = MD5Util.TextToMD5(vm.Password),
                RegisterTime = DateTime.Now,
                Status = (int)UserStatusEnum.UnActivated,
                IsDelete = (int)UserDeleteEnum.Normal,
                IsAdmin = (int)UserTypeEnum.IsUser
            };

            data.Data = await _usersProvider.AddUser(user);

            return data;
        }

        /// <summary>
        /// 编辑用户
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("edit_user")]
        public async Task<ResponseViewModel<bool>> EditUser(int userId, EditUserViewModel vm)
        {
            var data = new ResponseViewModel<bool>();

            var user = await _usersProvider.GetUserInfo(userId);
            user.UserName = vm.UserName;
            user.Email = vm.Email;
            user.Password = MD5Util.TextToMD5(vm.Password);

            data.Data = await _usersProvider.UpdateUser(user);

            return data;
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("del_user")]
        public async Task<ResponseViewModel<bool>> DeleteUser(int userId)
        {
            var data = new ResponseViewModel<bool>
            {
                Data = await _usersProvider.DeleteUser(userId)
            };

            return data;
        }

        /// <summary>
        /// 禁用用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("disable_user")]
        public async Task<ResponseViewModel<bool>> DisableUser(int userId)
        {
            var data = new ResponseViewModel<bool>
            {
                Data = await _usersProvider.DisableUser(userId)
            };

            return data;
        }
    }
}