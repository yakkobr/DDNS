﻿using DDNS.Provider.Tunnel;
using DDNS.Provider.Users;
using DDNS.Web.Filter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DDNS.Web.Controllers
{
    [Authorize]
    public class TunnelController : Controller
    {
        private readonly UsersProvider _usersProvider;
        private readonly TunnelProvider _tunnelProvider;
        public TunnelController(UsersProvider usersProvider, TunnelProvider tunnelProvider)
        {
            _usersProvider = usersProvider;
            _tunnelProvider = tunnelProvider;
        }

        /// <summary>
        /// 隧道列表
        /// </summary>
        /// <returns></returns>
        public IActionResult Index(int? userId)
        {
            if (userId == null)
            {
                return NotFound();
            }

            return View();
        }

        /// <summary>
        /// 开通隧道
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// 开通隧道，指定用户
        /// </summary>
        /// <returns></returns>
        [PermissionFilter]
        public async Task<IActionResult> AdminCreate(int userId)
        {
            var user = await _usersProvider.GetUserInfo(userId);
            return View(user);
        }

        /// <summary>
        /// 审核申请隧道
        /// </summary>
        /// <returns></returns>
        [PermissionFilter]
        public IActionResult Audit()
        {
            return View();
        }

        /// <summary>
        /// 申请隧道详情
        /// </summary>
        /// <param name="tunnelId"></param>
        /// <returns></returns>
        [PermissionFilter]
        public async Task<IActionResult> AuditDetail(string tunnelId)
        {
            var tunnel = await _tunnelProvider.GetTunnel(tunnelId);

            return View(tunnel);
        }
    }
}