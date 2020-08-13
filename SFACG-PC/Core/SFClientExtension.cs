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
        public static async Task<string> GetChapContent(this SFClient _, int ChapID) {
            var result = await HttpClientFactory.AppApiService().GetChapDetailResponse(ChapID.ToString());
            return result.data.expand.content;
        }

        public static async Task<List<Volumelist>> GetBookDir(this SFClient _, int NovelID) {
            var result = await HttpClientFactory.AppApiService().GetNovelDirResponse(NovelID.ToString());
            var list = new List<Volumelist>();

            if (result is { } res) {
                list.AddRange(res.data.volumeList.Select(p => new Volumelist {
                    Chapterlist = p.chapterList.ToChapterItem(),
                    Sno = p.sno,
                    Title = p.title,
                    VolumeId = p.volumeId
                }));
            }
            return list;
        }

        public static async Task<BookInfo> GetBookInfo(this SFClient _, string NovelID) {
            var result = await HttpClientFactory.AppApiService().GetNovelInfoResponse(NovelID);
            return new BookInfo() {
                AuthorName = result.data.authorName,
                ImgUrl = result.data.novelCover,
                AuthorUrl = await GetAuthorAvatar(_, result.data.authorId),
                Intro = result.data.expand.intro,
                IsNeedVIP = result.data.signStatus.ToLower().IndexOf("vip") >= 0,
                LatestString = result.data.expand.latestChapter.addTime.ToString() + "    " + result.data.expand.latestChapter.title,
                MarkCount = result.data.markCount.ToString(),
                Point = (int)(result.data.point / 2),
                TicketCount = result.data.expand.ticket.ToString(),
                Title = result.data.novelName,
                TypeString = result.data.expand.typeName + "/" + ((!result.data.isFinish) ? "连载中" : "已完结")
            };
        }
        public static async Task<string> GetAuthorAvatar(this SFClient _, int AuthorID) {
            var result = await HttpClientFactory.AppApiService().GetAuthorInfo(AuthorID.ToString());
            return result.data.avatar;
        }
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
                    Tags = book.expand.sysTags.ToTags(),
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
                    Tags = book.expand.sysTags.ToTags(),
                    NovelID = book.novelId,
                    AuthorName = book.authorName,
                    charCount = book.charCount.ToString(),
                    IsSerialize = !book.isFinish,
                    IsSign = (book.signStatus == "签约")
                }));
            }
            return list;
        }

        public static List<tag> ToTags<T>(this T[] systags) where T : Systag{
            var tags = new List<tag>();
            if (systags.IsNullOrEmpty()) return tags;
            tags.AddRange(systags.Select(tagitem => new tag() {
                tagName = tagitem.tagName,
                sysTagId = tagitem.sysTagId
            }));
            return tags;
        }

        public static List<ChapterItem> ToChapterItem<T>(this T[] chapterlist) where T : NovelDirResponse.Chapterlist {
            var result = new List<ChapterItem>();
            result.AddRange(chapterlist.Select(p => new ChapterItem() {
                ChapId = p.chapId,
                CharCount = p.charCount,
                IsVip = p.isVip,
                NeedFireMoney = p.needFireMoney,
                NovelId = p.novelId,
                Title = p.title,
                UpdateTime = p.updateTime,
                VolumeId = p.volumeId
            }));
            return result;
        }
    }
}
