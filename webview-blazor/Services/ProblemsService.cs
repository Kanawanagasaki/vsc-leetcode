using Kanawanagasaki.VSCode.LeetCode.WebView.Models;

namespace Kanawanagasaki.VSCode.LeetCode.WebView.Services;

public class ProblemsService : IDisposable
{
    public bool IsRunningCode { get; private set; } = false;
    public bool IsSubmitting { get; private set; } = false;
    public event Action? OnBeforeSubmit;

    private JsService _js;
    private Dictionary<Guid, TaskCompletionSource<ProblemSetModel>> _problemSetMap = new();
    private Dictionary<string, ProblemSetModel> _cache = new();
    private Queue<string> _cacheQueue = new();

    private Dictionary<Guid, TaskCompletionSource<DiscussionReplyModel[]?>> _discussionReplyMap = new();
    
    private Dictionary<Guid, TaskCompletionSource<CommunitySolutionCommentReplyModel[]?>> _communitySolutionCommentReplyMap = new();

    public ProblemsService(JsService js)
    {
        _js = js;
        JsService.OnProblemSet += JsService_OnProblemSet;
        JsService.OnDiscussionReplies += JsService_OnDiscussionReplies;
        JsService.OnCommunitySolutionCommentReply += JsService_OnCommunitySolutionCommentReply;
        JsService.OnRuncodeStateUpdate += JsService_OnRuncodeStateUpdate;
        JsService.OnSubmissionStateUpdate += JsService_OnSubmissionStateUpdate;
    }

    private void JsService_OnProblemSet(UuidResult<ProblemSetModel>? result)
    {
        if (result is null || !_problemSetMap.ContainsKey(result.Uuid))
        {
            foreach (var kv in _problemSetMap)
                kv.Value.TrySetResult(new() { Total = 0, Questions = new ProblemSetItemModel[0] });
            _problemSetMap.Clear();

            return;
        }

        _problemSetMap[result.Uuid].TrySetResult(result.Result);
        _problemSetMap.Remove(result.Uuid);
    }

    public async Task<ProblemSetModel> GetProblems(string categorySlug, int page, FilterModel filters, bool getCache)
    {
        var cacheKey = $"{categorySlug}.{page}.{filters.ToString()}";

        if (getCache && _cache.ContainsKey(cacheKey))
            return _cache[cacheKey];

        var uuid = Guid.NewGuid();
        var tcs = new TaskCompletionSource<ProblemSetModel>();
        _problemSetMap[uuid] = tcs;
        await _js.RequestProblems(categorySlug, page, filters, uuid, getCache);
        _cache[cacheKey] = await tcs.Task;
        _cacheQueue.Enqueue(cacheKey);

        if (_cacheQueue.Count > 128)
            _cache.Remove(_cacheQueue.Dequeue());

        return _cache[cacheKey];
    }

    public async Task<DiscussionReplyModel[]?> GetDiscussionReplies(long commentId)
    {
        var uuid = Guid.NewGuid();
        var tcs = new TaskCompletionSource<DiscussionReplyModel[]?>();
        _discussionReplyMap[uuid] = tcs;
        await _js.RequestDiscussionReplies(commentId, uuid);
        return await tcs.Task;
    }

    private void JsService_OnDiscussionReplies(UuidResult<DiscussionReplyModel[]>? result)
    {
        if (result is null || !_discussionReplyMap.ContainsKey(result.Uuid))
        {
            foreach (var kv in _discussionReplyMap)
                kv.Value.TrySetResult(null);
            _discussionReplyMap.Clear();

            return;
        }

        _discussionReplyMap[result.Uuid].TrySetResult(result.Result);
        _discussionReplyMap.Remove(result.Uuid);
    }

    public async Task<CommunitySolutionCommentReplyModel[]?> GetCommunitySolutionCommentReplies(long commentId)
    {
        var uuid = Guid.NewGuid();
        var tcs = new TaskCompletionSource<CommunitySolutionCommentReplyModel[]?>();
        _communitySolutionCommentReplyMap[uuid] = tcs;
        await _js.RequestCommunitySolutionCommentReplies(commentId, uuid);
        return await tcs.Task;
    }

    private void JsService_OnCommunitySolutionCommentReply(UuidResult<CommunitySolutionCommentReplyModel[]>? result)
    {
        if (result is null || !_communitySolutionCommentReplyMap.ContainsKey(result.Uuid))
        {
            foreach (var kv in _communitySolutionCommentReplyMap)
                kv.Value.TrySetResult(null);
            _communitySolutionCommentReplyMap.Clear();

            return;
        }

        _communitySolutionCommentReplyMap[result.Uuid].TrySetResult(result.Result);
        _communitySolutionCommentReplyMap.Remove(result.Uuid);
    }

    private void JsService_OnRuncodeStateUpdate(RuncodeStateUpdateModel? state)
    {
        if (state?.State.State != "PENDING" && state?.State.State != "STARTED")
            IsRunningCode = false;
    }

    private void JsService_OnSubmissionStateUpdate(SubmissionStateUpdateModel? state)
    {
        if (state?.State.State != "PENDING" && state?.State.State != "STARTED")
        {
            IsRunningCode = false;
            IsSubmitting = false;
        }
    }

    public async Task Run(string titleSlug, string dataInput)
    {
        if (IsRunningCode)
            return;
        IsRunningCode = true;

        await _js.RunCode(titleSlug, dataInput);
    }

    public async Task Submit(string titleSlug)
    {
        if (IsRunningCode)
            return;
        IsRunningCode = true;
        IsSubmitting = true;

        OnBeforeSubmit?.Invoke();
        await _js.Submit(titleSlug);
    }

    public void Dispose()
    {
        foreach (var kv in _problemSetMap)
            kv.Value.TrySetResult(new() { Total = 0, Questions = new ProblemSetItemModel[0] });
        _problemSetMap.Clear();

        JsService.OnProblemSet -= JsService_OnProblemSet;
        JsService.OnDiscussionReplies -= JsService_OnDiscussionReplies;
        JsService.OnCommunitySolutionCommentReply -= JsService_OnCommunitySolutionCommentReply;
        JsService.OnRuncodeStateUpdate -= JsService_OnRuncodeStateUpdate;
        JsService.OnSubmissionStateUpdate -= JsService_OnSubmissionStateUpdate;
    }
}
