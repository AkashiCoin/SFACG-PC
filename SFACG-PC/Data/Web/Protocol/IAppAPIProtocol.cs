﻿using Refit;
using SFACGPC.Data.Web.Request;
using SFACGPC.Data.Web.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SFACGPC.Data.Web.Protocol {
    [Headers("Authorization: Basic YW5kcm9pZHVzZXI6MWEjJDUxLXl0Njk7KkFjdkBxeHE=")]
    public interface IAppAPIProtocol {
        [Get("/specialpush")]
        Task<SpecialPushResponse> GetSpecialPushResponse();

        [Get("/comicspecialpush")]
        Task<ComicsSpecialPushResponse> GetComicsSpecialPushResponse();

        [Get("/novels/0/sysTags?filter=push")]
        Task<SysTagsResponse> GetSysTagsResponse();

        [Get("/novels?size=8&filter=hotpush&expand=chapterCount,bigBgBanner,bigNovelCover,typeName,intro,fav,ticket,pointCount,tags,sysTags,signlevel,discount,discountExpireDate,totalNeedFireMoney,originTotalNeedFireMoney,latestchapter,latestcommentdate,essaytag,auditCover")]
        Task<NovelHotPushResponse> GetNovelHotPushResponse();

        [Get("/novels?page=0&size=6&filter=chatnovelpush&categoryId=1&expand=typeName%2Ctags%2Cintro")]
        Task<ChatNovelPushResponse> GetChatNovelPushResponse();

        [Get("/novels/{novelid}?expand=chapterCount,bigBgBanner,bigNovelCover,typeName,intro,fav,ticket,pointCount,tags,sysTags,signlevel,discount,discountExpireDate,totalNeedFireMoney,originTotalNeedFireMoney,latestchapter,latestcommentdate,essaytag,auditCover")]
        Task<NovelInfoResponse> GetNovelInfoResponse([AliasAs("novelid")] string NovelID);

        [Get("/novels/{novelid}/dirs?expand=originNeedFireMoney")]
        Task<NovelDirResponse> GetNovelDirResponse([AliasAs("novelid")]string NovelID);

        [Get("/comics?page=0&size=6&filter=hotpush&expand=typeName%2Cintro%2CauthorName%2Coriginalauthor%2Cdiscount%2CdiscountExpireDate%2CtotalNeedFireMoney%2CoriginTotalNeedFireMoney")]
        Task<ComicsHotPushResponse> GetComicsHotPushResponse();

        [Get("/Chaps/{chapid}?expand=content%2CneedFireMoney%2CoriginNeedFireMoney%2Ctsukkomi%2Cchatlines&autoOrder=false")]
        Task<ChapDetailResponse> GetChapDetailResponse([AliasAs("chapid")]string ChapID);

        [Get("/user?expand=vipInfo%2CwelfareCoin%2CisRealNameAuth%2CchangeNickNameInfo%2CwelfareMoney%2CredpacketCode%2CuseWelfaresys%2CusedRedpacketCode")]
        Task<UserInfoResponse> GetUserInfoResponse();

        [Get("/user/tickets?page=0&size=24&isexpired=not")]
        Task<UserTicketResponse> GetUserTicketResponse();

        [Get("/user/money")]
        Task<UserMoneyResponse> GetUserMoneyResponse();
    }
}
