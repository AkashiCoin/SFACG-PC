﻿using SFACGPC.Data.ViewModel;
using SFACGPC.Data.Web.Delegation;
using SFACGPC.Data.Web.Response;
using SFACGPC.Objects.Generic;
using SFACGPC.Objects.Primitive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SFACGPC.Data.Web.Response.PublicBookInfo;

namespace SFACGPC.Core {
    public static class SFClientExtension {

        public static async Task<List<SpecialPushItem>> GetComicsSpecialPush(this SFClient _) {
            var result = await HttpClientFactory.AppApiService().GetComicsSpecialPushResponse();
            var list = new List<SpecialPushItem>();
            if (result is { } res) {
                list.AddRange(res.data.comicPush.Select(book => new SpecialPushItem() {
                    ImgUrl = book.imgUrl,
                    Link = book.link,
                    Type = book.type
                }));
            }
            return list;
        }

        public static async Task<List<SpecialPushItem>> GetChatNovelSpecialPush(this SFClient _) {
            var result = await HttpClientFactory.AppApiService().GetSpecialPushResponse();
            var list = new List<SpecialPushItem>();
            if (result is { } res) {
                list.AddRange(res.data.chatNovelPush.Select(book => new SpecialPushItem() {
                    ImgUrl = book.imgUrl,
                    Link = book.link,
                    Type = book.type
                }));
            }
            return list;
        }

        public static async Task<List<SpecialPushItem>> GetSpecialPush(this SFClient _) {
            var result = await HttpClientFactory.AppApiService().GetSpecialPushResponse();
            var list = new List<SpecialPushItem>();
            if (result is { } res) {
                list.AddRange(res.data.homePush.Select(book => new SpecialPushItem() {
                    ImgUrl = book.imgUrl,
                    Link = book.link,
                    Type = book.type
                }));
            }
            return list;
        }

        public static async Task<List<HotPushItem>> GetNovelHotPush(this SFClient _) {
            var result = await HttpClientFactory.AppApiService().GetNovelHotPushResponse();
            var list = new List<HotPushItem>();
            if (result is { } res) {
                list.AddRange(res.data.Select(book => new HotPushItem() {
                    CoverUrl = book.novelCover,
                    Title = book.novelName,
                    Tags = Systag2tags(book.expand.sysTags),
                    NovelID = book.novelId,
                    AuthorName = book.authorName,
                    charCount = book.charCount.ToString(),
                    IsSerialize = !book.isFinish,
                    IsSign = (book.signStatus == "签约")
                }));
            }
            return list;
        }
        public static async Task<List<HotPushItem>> GetComicsHotPush(this SFClient _) {
            var result = await HttpClientFactory.AppApiService().GetComicsHotPushResponse();
            var list = new List<HotPushItem>();
            if (result is { } res) {
                list.AddRange(res.data.Select(book => new HotPushItem() {
                    CoverUrl = book.comicCover,
                    Title = book.comicName,
                    NovelID = book.comicId,
                    AuthorName = book.comicName,
                    IsSerialize = !book.isFinished,
                    IsSign = false
                }));
            }
            return list;
        }
        public static async Task<List<HotPushItem>> GetChatNovelHotPush(this SFClient _) {
            var result = await HttpClientFactory.AppApiService().GetChatNovelPushResponse();
            var list = new List<HotPushItem>();
            if (result is { } res) {
                list.AddRange(res.data.Select(book => new HotPushItem() {
                    CoverUrl = book.novelCover,
                    Title = book.novelName,
                    Tags = Systag2tags(book.expand.sysTags),
                    NovelID = book.novelId,
                    AuthorName = book.authorName,
                    charCount = book.charCount.ToString(),
                    IsSerialize = !book.isFinish,
                    IsSign = (book.signStatus == "签约")
                }));
            }
            return list;
        }

        public static List<tag> Systag2tags(Systag[] systags) {
            var tags = new List<tag>();
            if (systags.IsNullOrEmpty()) return tags;
            tags.AddRange(systags.Select(tagitem => new tag() {
                tagName = tagitem.tagName,
                sysTagId = tagitem.sysTagId
            }));
            return tags;
        }
    }
}
