using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Atlassian.Stash.Api;
using Atlassian.Stash.Entities;
using Atlassian.Stash.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Atlassian.Stash.IntegrationTests
{
    [TestClass]
    public class StashClientTester : TestBase
    {
        [TestMethod]
        public async Task Can_GetFileContents()
        {
            var response = await this.stashClient.Repositories.GetFileContents(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY, this.EXISTING_FILE);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.FileContents.Count > 0);
            Assert.AreEqual(1, response.Size);
        }

        [TestMethod]
        public async Task Can_GetFileContents_In_SubFolder()
        {
            var response = await this.stashClient.Repositories.GetFileContents(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY, this.EXISTING_FILE_IN_SUBFOLDER);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.FileContents.Count > 0);
            Assert.AreEqual(1, response.Size);
        }

        [TestMethod]
        public async Task Can_GetFileContents_In_SubFolder_With_Spaces()
        {
            var response = await this.stashClient.Repositories.GetFileContents(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY, this.EXISTING_FILE_IN_SUBFOLDER_WITH_SPACES);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.FileContents.Count > 0);
            Assert.AreEqual(1, response.Size);
        }

        [TestMethod]
        public async Task Can_GetFileContents_From_Branch()
        {
            var response = await this.stashClient.Repositories.GetFileContents(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY, this.EXISTING_FILE, this.MASTER_BRANCH_NAME);
            
            Assert.IsNotNull(response);
            Assert.IsTrue(response.FileContents.Count > 0);
            Assert.AreEqual(1, response.Size);
        }
        
        [TestMethod]
        public async Task Can_GetBranchesForCommit()
        {
            var response = await this.stashClient.Branches.GetByCommitId(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY, this.EXISTING_COMMIT);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Values.Any(x => x.Id.Equals(this.MASTER_BRANCH_REFERENCE)));
        }

        [TestMethod]
        public async Task Can_GetAllProjects()
        {
            var response = await this.stashClient.Projects.Get();
            var projects = response.Values;

            Assert.IsNotNull(projects);
            Assert.IsInstanceOfType(projects, typeof(IEnumerable<Project>));
            Assert.IsTrue(projects.Any());
        }

        [TestMethod]
        public async Task Can_GetAllUsers()
        {
            var response = await this.stashClient.Groups.GetUsers(this.EXISTING_GROUP, new RequestOptions { Limit = 1, Start = 0 });
            var projects = response.Values;

            Assert.IsNotNull(projects);
            Assert.IsInstanceOfType(projects, typeof(IEnumerable<User>));
            Assert.IsTrue(projects.Any());
        }

        [TestMethod]
        public async Task Can_GetAllProjects_WithRequestOptions()
        {
            var requestLimit = 1;
            var response = await this.stashClient.Projects.Get(new RequestOptions { Limit = requestLimit, Start = 0 });
            var projects = response.Values;

            Assert.IsNotNull(projects);
            Assert.IsInstanceOfType(projects, typeof(IEnumerable<Project>));
            Assert.AreEqual(requestLimit, projects.Count());
        }

        [TestMethod]
        public async Task Can_GetByIdProject()
        {
            var project = await this.stashClient.Projects.GetById(this.EXISTING_PROJECT);

            Assert.IsNotNull(project);
            Assert.IsInstanceOfType(project, typeof(Project));
            Assert.AreEqual(this.EXISTING_PROJECT.ToLower(), project.Name.ToLower());
        }

        [TestMethod]
        public async Task Can_GetAllSizes()
        {
            var response = await this.stashClient.Repositories.GetSizes(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY);

            Assert.IsNotNull(response.Repository);
            Assert.IsNotNull(response.Attachments);
        }

        [TestMethod]
        public async Task Can_GetAllRepositories()
        {
            var response = await this.stashClient.Repositories.Get(this.EXISTING_PROJECT);
            var repositories = response.Values;

            Assert.IsNotNull(repositories);
            Assert.IsInstanceOfType(repositories, typeof(IEnumerable<Repository>));
            Assert.IsTrue(repositories.Any());
        }

        [TestMethod]
        public async Task Can_GetAllRepositories_WithRequestOptions()
        {
            var requestLimit = 1;
            var response = await this.stashClient.Repositories.Get(this.EXISTING_PROJECT, new RequestOptions { Limit = requestLimit });
            var repositories = response.Values;

            Assert.IsNotNull(repositories);
            Assert.IsInstanceOfType(repositories, typeof(IEnumerable<Repository>));
            Assert.AreEqual(requestLimit, repositories.Count());
        }

        [TestMethod]
        public async Task Can_GetByIdRepository()
        {
            var repository = await this.stashClient.Repositories.GetById(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY);

            Assert.IsNotNull(repository);
            Assert.IsInstanceOfType(repository, typeof(Repository));
            Assert.AreEqual(this.EXISTING_REPOSITORY.ToLower(), repository.Name.ToLower());
        }

        [TestMethod]
        public async Task Can_GetAllTags()
        {
            var response = await this.stashClient.Repositories.GetTags(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY);
            var tags = response.Values;

            Assert.IsNotNull(tags);
            Assert.IsInstanceOfType(tags, typeof(IEnumerable<Tag>));
            Assert.IsTrue(tags.Any());
        }

        [TestMethod]
        public async Task Can_GetAllTags_WithRequestOptions()
        {
            var requestLimit = 1;
            var response = await this.stashClient.Repositories.GetTags(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY, new RequestOptions { Limit = requestLimit });
            var tags = response.Values;

            Assert.IsNotNull(tags);
            Assert.IsInstanceOfType(tags, typeof(IEnumerable<Tag>));
            Assert.AreEqual(requestLimit, tags.Count());
        }

        [TestMethod]
        public async Task Can_Create_And_Delete_Tags()
        {
            var initialResponse = await this.stashClient.Repositories.GetTags(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY);
            var initialTagCount = initialResponse.Values.Count();

            // create tag
            var createTag = new Tag
            {
                Force = true,
                Name = "integration-test-tag",
                Message = "integration test tag",
                StartPoint = "refs/heads/master",
                Type = TagType.ANNOTATED
            };
            var createResponse = await this.stashClient.Repositories.CreateTag(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY, createTag);

            // mid-step get tags again
            var midResponse = await this.stashClient.Repositories.GetTags(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY);
            var midTagCount = midResponse.Values.Count();
            Assert.AreEqual(initialTagCount + 1, midTagCount);
            Assert.IsTrue(midResponse.Values.Any(x => x.Id.Contains(createTag.Name)));

            // delete tag
            await this.stashClient.Repositories.DeleteTag(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY, createTag.Name);

            // final check to ensure the tag count didn't change
            var finalResponse = await this.stashClient.Repositories.GetTags(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY);
            var finalTagCount = initialResponse.Values.Count();

            Assert.AreEqual(initialTagCount, finalTagCount);
        }

        [TestMethod]
        public async Task Can_GetFilesInSubFolder()
        {
            var response = await this.stashClient.Repositories.GetFiles(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY, this.EXISTING_FOLDER);
            var files = response.Values;

            Assert.IsNotNull(files);
            Assert.IsInstanceOfType(files, typeof(IEnumerable<string>));
            Assert.IsTrue(files.Any());
        }

        [TestMethod]
        public async Task Can_GetAllFiles()
        {
            var response = await this.stashClient.Repositories.GetFiles(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY);
            var files = response.Values;

            Assert.IsNotNull(files);
            Assert.IsInstanceOfType(files, typeof(IEnumerable<string>));
            Assert.IsTrue(files.Any());
        }

        [TestMethod]
        public async Task Can_GetAllFiles_WithRequestOptions()
        {
            var requestLimit = 1;
            var response = await this.stashClient.Repositories.GetFiles(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY, new RequestOptions { Limit = requestLimit });
            var files = response.Values;

            Assert.IsNotNull(files);
            Assert.IsInstanceOfType(files, typeof(IEnumerable<string>));
            Assert.AreEqual(requestLimit, files.Count());
        }

        [TestMethod]
        public async Task Can_GetPullRequestSettings()
        {
            var response = await this.stashClient.Repositories.GetPullRequestSettings(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY);

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.MergeConfig);
            Assert.IsNotNull(response.MergeConfig.DefaultStrategy);
            Assert.IsNotNull(response.MergeConfig.Strategies);
            Assert.IsNotNull(response.MergeConfig.Type = "REPOSITORY");
            Assert.IsNotNull(response.RequiredApprovers = 0);

        }

        [TestMethod]
        public async Task Can_SetPullRequestSettings()
        {
            var originalSettings = await this.stashClient.Repositories.GetPullRequestSettings(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY);

            var settings = new PullRequestSettings
            {
                MergeConfig = new MergeConfig
                {
                    DefaultStrategy = new Strategy {Id = StrategyIdType.SQUASH_FAST_FORWARD_ONLY},
                    Strategies = new List<Strategy>
                    {
                        new Strategy {Id = StrategyIdType.SQUASH_FAST_FORWARD_ONLY},
                        new Strategy {Id = StrategyIdType.NO_FAST_FORWARD}
                    }
                },
                RequiredAllApprovers = false,
                RequiredAllTasksComplete = false,
                RequiredApprovers = 2,
                RequiredSuccessfulBuilds = 1
            };

            await this.stashClient.Repositories.SetPullRequestSettings(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY, settings);

            var response = await this.stashClient.Repositories.GetPullRequestSettings(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY);

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.MergeConfig);
            Assert.IsNotNull(response.MergeConfig.DefaultStrategy);
            Assert.IsNotNull(response.MergeConfig.DefaultStrategy.Id = StrategyIdType.SQUASH_FAST_FORWARD_ONLY);
            Assert.IsNotNull(response.MergeConfig.Strategies);
            Assert.AreEqual(2, response.MergeConfig.Strategies.Count(s => s.Enabled != null && s.Enabled.Value));
            Assert.AreEqual(2, response.RequiredApprovers);
            Assert.AreEqual(1, response.RequiredSuccessfulBuilds);

            await this.stashClient.Repositories.SetPullRequestSettings(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY, originalSettings);

        }

        [TestMethod]
        public async Task Can_GetAllBranches()
        {
            var response = await this.stashClient.Branches.Get(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY);
            var branches = response.Values;

            Assert.IsNotNull(branches);
            Assert.IsInstanceOfType(branches, typeof(IEnumerable<Branch>));
            Assert.IsTrue(branches.Any());
        }

        [TestMethod]
        public async Task GetPullRequest_RetrieveAllPullRequests_ReturnsSomePullRequests()
        {
            var response = await this.stashClient.PullRequests.Get(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY, state: PullRequestState.ALL);
            var pullRequests = response.Values;

            Assert.IsNotNull(pullRequests);
            Assert.IsInstanceOfType(pullRequests, typeof(IEnumerable<PullRequest>));
            Assert.IsTrue(pullRequests.Any());
        }

        [TestMethod]
        public async Task GetPullRequest_WithRequestOptions_ReturnsSomePullRequests()
        {
            var response = await this.stashClient.PullRequests.Get(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY, new RequestOptions { Limit = 1 }, state: PullRequestState.ALL);
            var pullRequests = response.Values;

            Assert.IsNotNull(pullRequests);
            Assert.IsInstanceOfType(pullRequests, typeof(IEnumerable<PullRequest>));
            Assert.IsTrue(pullRequests.Any());
        }

        [TestMethod]
        public async Task Can_GetAllBranches_WithRequestOptions()
        {
            var requestLimit = 1;
            var response = await this.stashClient.Branches.Get(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY, new RequestOptions { Limit = requestLimit });
            var branches = response.Values;

            Assert.IsNotNull(branches);
            Assert.IsInstanceOfType(branches, typeof(IEnumerable<Branch>));
            Assert.AreEqual(requestLimit, branches.Count());
        }

        [TestMethod]
        public async Task Can_GetAllCommits()
        {
            var response = await this.stashClient.Commits.Get(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY);
            var commits = response.Values;

            Assert.IsNotNull(commits);
            Assert.IsInstanceOfType(commits, typeof(IEnumerable<Commit>));
            Assert.IsTrue(commits.Any());
        }

        [TestMethod]
        public async Task Can_GetAllCommits_WithRequestOptions()
        {
            var requestLimit = 2;
            var response = await this.stashClient.Commits.Get(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY, new RequestOptions { Limit = requestLimit });
            var commits = response.Values;

            Assert.IsNotNull(commits);
            Assert.IsInstanceOfType(commits, typeof(IEnumerable<Commit>));
            Assert.AreEqual(requestLimit, commits.Count());
        }

        [TestMethod]
        public async Task Can_GetAllCommits_WithRequestOptionsForCommits()
        {
            var expectedCommitCount = 1;
            var response = await this.stashClient.Commits.Get(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY, null, new RequestOptionsForCommits { Until = EXISTING_COMMIT, Since = EXISTING_OLDER_COMMIT });
            var commits = response.Values;

            Assert.IsNotNull(commits);
            Assert.IsInstanceOfType(commits, typeof(IEnumerable<Commit>));
            Assert.AreEqual(expectedCommitCount, commits.Count());
        }

        [TestMethod]
        public async Task Can_GetByIdCommit()
        {
            var commit = await this.stashClient.Commits.GetById(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY, this.EXISTING_COMMIT);

            Assert.IsNotNull(commit);
            Assert.IsInstanceOfType(commit, typeof(Commit));
            Assert.AreEqual(this.EXISTING_COMMIT.ToLower(), commit.Id.ToLower());
        }

        [TestMethod]
        public async Task Can_GetChangesUntil()
        {
            var changes = await this.stashClient.Commits.GetChanges(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY, this.EXISTING_COMMIT);

            Assert.IsNotNull(changes);
            Assert.IsInstanceOfType(changes, typeof(Changes));
            Assert.AreEqual(this.EXISTING_COMMIT.ToLower(), changes.ToHash.ToLower());
        }

        [TestMethod]
        public async Task Can_GetChangesUntil_WithRequestOptions()
        {
            var requestLimit = 1;
            var changes = await this.stashClient.Commits.GetChanges(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY, this.EXISTING_COMMIT, null, new RequestOptions { Limit = requestLimit });

            Assert.IsNotNull(changes);
            Assert.IsInstanceOfType(changes, typeof(Changes));
            Assert.AreEqual(this.EXISTING_COMMIT.ToLower(), changes.ToHash.ToLower());
            Assert.AreEqual(requestLimit, changes.ListOfChanges.Count());
        }

        [TestMethod]
        public async Task Can_GetChangesUntil_And_Since()
        {
            var changes = await this.stashClient.Commits.GetChanges(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY, this.EXISTING_COMMIT, this.EXISTING_OLDER_COMMIT);

            Assert.IsNotNull(changes);
            Assert.IsInstanceOfType(changes, typeof(Changes));
            Assert.AreEqual(this.EXISTING_COMMIT.ToLower(), changes.ToHash.ToLower());
        }

        [TestMethod]
        public async Task Can_GetChangesUntil_And_Since_WithRequestOptions()
        {
            var requestLimit = 1;
            var changes = await this.stashClient.Commits.GetChanges(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY, this.EXISTING_COMMIT, this.EXISTING_OLDER_COMMIT, new RequestOptions { Limit = requestLimit });

            Assert.IsNotNull(changes);
            Assert.IsInstanceOfType(changes, typeof(Changes));
            Assert.AreEqual(this.EXISTING_COMMIT.ToLower(), changes.ToHash.ToLower());
            Assert.AreEqual(requestLimit, changes.ListOfChanges.Count());
        }

        [TestMethod]
        public async Task Can_GetChangesUntil_And_Since_MoreThanOneResult()
        {
            var changes = await this.stashClient.Commits.GetChanges(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY, this.EXISTING_COMMIT, this.EXISTING_OLDER_COMMIT);

            Assert.IsNotNull(changes);
            Assert.IsInstanceOfType(changes, typeof(Changes));
            Assert.AreEqual(this.EXISTING_COMMIT, changes.ToHash, ignoreCase: true);
            Assert.AreEqual(this.EXISTING_NUMBER_OF_CHANGES, changes.ListOfChanges.Count());
        }

        [TestMethod]
        public async Task Can_GetCommitsUntil()
        {
            var commits = await this.stashClient.Commits.GetCommits(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY, this.EXISTING_COMMIT);

            Assert.IsNotNull(commits);
            Assert.IsInstanceOfType(commits, typeof(ResponseWrapper<Commit>));
            Assert.IsTrue(commits.Values.Count() > 1);
            Assert.IsTrue(commits.Values.Any(x => x.Id.Equals(this.EXISTING_COMMIT, StringComparison.OrdinalIgnoreCase)));
        }

        [TestMethod]
        public async Task Can_GetCommitsUntil_WithRequestOptions()
        {
            var requestLimit = 1;
            var commits = await this.stashClient.Commits.GetCommits(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY, this.EXISTING_COMMIT, null, new RequestOptions { Limit = requestLimit });

            Assert.IsNotNull(commits);
            Assert.IsInstanceOfType(commits, typeof(ResponseWrapper<Commit>));
            Assert.IsTrue(commits.Values.Count() > 0);
            Assert.IsTrue(commits.Values.Any(x => x.Id.Equals(this.EXISTING_COMMIT, StringComparison.OrdinalIgnoreCase)));
        }

        [TestMethod]
        public async Task Can_GetCommitsUntil_And_Since()
        {
            var commits = await this.stashClient.Commits.GetCommits(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY, this.EXISTING_COMMIT, this.EXISTING_OLDER_COMMIT);

            Assert.IsNotNull(commits);
            Assert.IsInstanceOfType(commits, typeof(ResponseWrapper<Commit>));
            Assert.IsTrue(commits.Values.Count() > 0);
            Assert.IsTrue(commits.Values.Any(x => x.Id.Equals(this.EXISTING_COMMIT, StringComparison.OrdinalIgnoreCase)));
            // excluside call (excludes 'since' commit)
            Assert.IsFalse(commits.Values.Any(x => x.Id.Equals(this.EXISTING_OLDER_COMMIT, StringComparison.OrdinalIgnoreCase)));
        }

        [TestMethod]
        public async Task Can_GetCommitsUntil_And_Since_WithRequestOptions()
        {
            var requestLimit = 1;
            var commits = await this.stashClient.Commits.GetCommits(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY, this.EXISTING_COMMIT, this.EXISTING_OLDER_COMMIT, new RequestOptions { Limit = requestLimit });

            Assert.IsNotNull(commits);
            Assert.IsInstanceOfType(commits, typeof(ResponseWrapper<Commit>));
            Assert.IsTrue(commits.Values.Count() > 0);
            Assert.IsTrue(commits.Values.Any(x => x.Id.Equals(this.EXISTING_COMMIT, StringComparison.OrdinalIgnoreCase)));
            // excluside call (excludes 'since' commit)
            Assert.IsFalse(commits.Values.Any(x => x.Id.Equals(this.EXISTING_OLDER_COMMIT, StringComparison.OrdinalIgnoreCase)));
        }

        [TestMethod]
        public async Task Can_GetCommitsUntil_And_Since_MoreThanOneResult()
        {
            var commits = await this.stashClient.Commits.GetCommits(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY, this.EXISTING_COMMIT, this.EXISTING_OLDER_COMMIT);

            Assert.IsNotNull(commits);
            Assert.IsInstanceOfType(commits, typeof(ResponseWrapper<Commit>));
            Assert.IsTrue(commits.Values.Count() > 0);
            Assert.IsTrue(commits.Values.Any(x => x.Id.Equals(this.EXISTING_COMMIT, StringComparison.OrdinalIgnoreCase)));
            // excluside call (excludes 'since' commit)
            Assert.IsFalse(commits.Values.Any(x => x.Id.Equals(this.EXISTING_OLDER_COMMIT, StringComparison.OrdinalIgnoreCase)));
        }

        [TestMethod]
        public async Task Can_GetRepository_Hooks()
        {
            var response = await this.stashClient.Repositories.GetHooks(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY);
            var hooks = response.Values;

            Assert.IsNotNull(hooks);
            Assert.IsInstanceOfType(hooks, typeof(IEnumerable<Hook>));
            Assert.IsTrue(hooks.Any());
        }

        [TestMethod]
        public async Task Can_GetRepository_Hooks_WithRequestOptions()
        {
            var requestLimit = 1;
            var response = await this.stashClient.Repositories.GetHooks(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY, new RequestOptions { Limit = requestLimit });
            var hooks = response.Values;

            Assert.IsNotNull(hooks);
            Assert.IsInstanceOfType(hooks, typeof(IEnumerable<Hook>));
            Assert.AreEqual(requestLimit, hooks.Count());
        }

        [TestMethod]
        public async Task Can_GetRepository_Hook_ById()
        {
            var response = await this.stashClient.Repositories.GetHookById(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY, this.EXISTING_HOOK);

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(Hook));
            Assert.AreEqual(this.EXISTING_HOOK, response.Details.Key);
        }

        [TestMethod]
        public async Task Can_SetDefaultBranch_Than_RetrieveIt()
        {
            var response = await this.stashClient.Branches.GetDefault(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY);

            Assert.IsNotNull(response);
            Assert.AreEqual("refs/heads/master", response.Id);

            var defaultBranch = new Branch { Id = "refs/heads/develop" };

            await this.stashClient.Branches.SetDefault(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY, defaultBranch);

            response = await this.stashClient.Branches.GetDefault(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY);

            Assert.IsNotNull(response);
            Assert.AreEqual("refs/heads/develop", response.Id);

            defaultBranch = new Branch { Id = "refs/heads/master" };

            await this.stashClient.Branches.SetDefault(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY, defaultBranch);
        }

        [TestMethod]
        public async Task Can_GetBranchPermissions()
        {
            var response = await this.stashClient.Branches.GetPermissions(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY);

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(ResponseWrapper<BranchPermission>));
        }

        [TestMethod]
        public async Task Can_SetBranchPermissions_Than_DeleteBranchPermissions()
        {
            var setBranchPerm = new BranchPermission
            {
                Type = BranchPermissionType.READ_ONLY,
                Matcher = new BranchPermissionMatcher
                {
                    Id = "master",
                    DisplayId = "master",
                    Active = true,
                    Type = new BranchPermissionMatcherType
                    {
                        Id = BranchPermissionMatcherTypeName.BRANCH,
                        Name = "Branch"
                    }
                },
                Users = new List<User>(),
                Groups = new string[] { this.EXISTING_GROUP }
            };

            var response = await this.stashClient.Branches.SetPermissions(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY, setBranchPerm);

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(BranchPermission));
            Assert.AreEqual(setBranchPerm.Type, response.Type);
            Assert.AreEqual(setBranchPerm.Matcher.Id, response.Matcher.Id);
            Assert.AreEqual(setBranchPerm.Matcher.Type.Id, response.Matcher.Type.Id);
            Assert.IsTrue(response.Id > 0);

            await this.stashClient.Branches.DeletePermissions(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY, response.Id);
        }

        [TestMethod]
        public async Task Can_SetBranchPermissions_Than_DeleteBranchPermissions_Using_Pattern()
        {
            var setBranchPerm = new BranchPermission
            {
                Type = BranchPermissionType.READ_ONLY,
                Matcher = new BranchPermissionMatcher
                {
                    Id = "**",
                    DisplayId = "**",
                    Active = true,
                    Type = new BranchPermissionMatcherType
                    {
                        Id = BranchPermissionMatcherTypeName.PATTERN,
                        Name = "Pattern"
                    }
                },
                Users = new List<User>(),
                Groups = new string[] { this.EXISTING_GROUP }
            };

            var response = await this.stashClient.Branches.SetPermissions(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY, setBranchPerm);

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(BranchPermission));
            Assert.AreEqual(setBranchPerm.Type, response.Type);
            Assert.AreEqual(setBranchPerm.Matcher.Id, response.Matcher.Id);
            Assert.AreEqual(setBranchPerm.Matcher.Type.Id, response.Matcher.Type.Id);
            Assert.IsTrue(response.Id > 0);

            await this.stashClient.Branches.DeletePermissions(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY, response.Id);
        }

        [TestMethod]
        public async Task Can_CreateProject_Than_DeleteProject()
        {
            var newProject = new Project { Key = "ZTEST", Name = "Project of Integration tests", Description = "Project created by integration tests, please delete!" };
            var createdProject = await this.stashClient.Projects.Create(newProject);

            Assert.IsNotNull(createdProject);
            Assert.IsInstanceOfType(createdProject, typeof(Project));
            Assert.AreEqual(newProject.Key.ToLower(), createdProject.Key.ToLower());

            await this.stashClient.Projects.Delete(newProject.Key);
        }

        [TestMethod]
        public async Task Can_CreateRepository_Than_DeleteRepository()
        {
            var newRepository = new Repository { Name = "Repository of Integration tests" };
            var createdRepository = await this.stashClient.Repositories.Create(this.EXISTING_PROJECT, newRepository);

            Assert.IsNotNull(createdRepository);
            Assert.IsInstanceOfType(createdRepository, typeof(Repository));
            Assert.AreEqual(newRepository.Name.ToLower(), createdRepository.Name.ToLower());

            await this.stashClient.Repositories.Delete(this.EXISTING_PROJECT, createdRepository.Slug);
        }

        [TestMethod]
        public async Task Can_CreateBranch_Than_DeleteBranch()
        {
            var newBranch = new Branch { Name = "test-repo", StartPoint = MASTER_BRANCH_REFERENCE };
            var createdBranch = await this.stashClient.Branches.Create(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY, newBranch);

            Assert.IsNotNull(createdBranch);
            Assert.IsInstanceOfType(createdBranch, typeof(Branch));
            Assert.AreEqual(newBranch.Name.ToLower(), createdBranch.DisplayId.ToLower());


            var deleteBranch = new Branch { Name = newBranch.Name, DryRun = false };

            await this.stashClient.Branches.Delete(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY, deleteBranch);
        }

        [TestMethod]
        public async Task Can_Get_Enable_And_Disable_Hook()
        {
            var enableHook = await this.stashClient.Repositories.EnableHook(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY, this.EXISTING_HOOK);

            Assert.IsNotNull(enableHook);
            Assert.IsTrue(enableHook.Enabled);
            Assert.IsInstanceOfType(enableHook, typeof(Hook));
            Assert.AreEqual(this.EXISTING_HOOK, enableHook.Details.Key);

            var disableHook = await this.stashClient.Repositories.DisableHook(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY, this.EXISTING_HOOK);

            Assert.IsNotNull(disableHook);
            Assert.IsFalse(disableHook.Enabled);
            Assert.IsInstanceOfType(disableHook, typeof(Hook));
            Assert.AreEqual(this.EXISTING_HOOK, disableHook.Details.Key);
        }

        [TestMethod]
        public async Task Can_Create_Group_Then_Delete_Group()
        {
            #region Setup/Clean up
            var existingTestGroups = await this.stashClient.Groups.Get("tempTestGroup");

            foreach (var existingGroup in existingTestGroups.Values)
            {
                await this.stashClient.Groups.Delete(existingGroup.Name);
            }
            #endregion

            var group = await this.stashClient.Groups.Create("tempTestGroup");

            await this.stashClient.Groups.Delete(group.Name);

            var finalSetOfTestGroups = await this.stashClient.Groups.Get("tempTestGroup");

            Assert.AreEqual(0, finalSetOfTestGroups.Values.Count());
        }

        [TestMethod]
        public async Task Can_Create_User_Then_AddTogroup_Then_Delete_User()
        {
            #region Setup/Clean up
            var existingTestUsers = await this.stashClient.Users.Get("tmpTestUser");

            foreach (var existingUser in existingTestUsers.Values)
            {
                await this.stashClient.Users.Delete(existingUser.Name);
            }
            #endregion

            await this.stashClient.Users.Create("tmpTestUser", "Temporary test user", "tmpUser@company.com", "password");

            await this.stashClient.Users.AddToGroups("tmpTestUser", this.EXISTING_GROUP);

            var deletedUser = await this.stashClient.Users.Delete("tmpTestUser");

            Assert.IsNotNull(deletedUser);
            Assert.IsInstanceOfType(deletedUser, typeof(User));
            Assert.AreEqual("tmpTestUser", deletedUser.Name);
        }

        [TestMethod]
        public async Task Can_Get_Then_Create_Then_Grant_Access_To_Project_And_Delete_User()
        {
            #region Setup/Clean up
            var existingTestUsers = await this.stashClient.Users.Get("tmpTestUser");

            foreach (var existingUser in existingTestUsers.Values)
            {
                await this.stashClient.Users.Delete(existingUser.Name);
            }
            #endregion

            await this.stashClient.Users.Create("tmpTestUser", "Temporary test user", "tmpUser@company.com", "password");

            await this.stashClient.Projects.GrantUser(this.EXISTING_PROJECT, "tmpTestUser", ProjectPermissions.PROJECT_ADMIN);

            var deletedUser = await this.stashClient.Users.Delete("tmpTestUser");

            Assert.IsNotNull(deletedUser);
            Assert.IsInstanceOfType(deletedUser, typeof(User));
            Assert.AreEqual("tmpTestUser", deletedUser.Name);
        }

        [TestMethod]
        public async Task Can_GetPullRequestStatus()
        {
            var pullrequests = await this.stashClient.PullRequests.Get(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY);
            var request = pullrequests.Values.First();
            var status = await this.stashClient.PullRequests.Status(request, this.EXISTING_PROJECT);

            Assert.IsNotNull(status);
        }

        [TestMethod]
        public async Task Create_Update_Decline_Approve_Merge_PullRequest()
        {
            var newPullRequest = new PullRequest
            {
                Title = "Automatically Generated Pull Request",
                Author = new AuthorWrapper
                {
                    User = new Author
                    {
                        Name = TEST_USERNAME
                    }
                },
                FromRef = new Ref
                {
                    Id = DEVELOP_BRANCH_REFERENCE,
                    Repository = new Repository
                    {
                        Slug = EXISTING_REPOSITORY,
                        Project = new Project
                        {
                            Key = EXISTING_PROJECT
                        }
                    }
                },
                ToRef = new Ref
                {
                    Id = MASTER_BRANCH_REFERENCE,
                    Repository = new Repository
                    {
                        Slug = EXISTING_REPOSITORY,
                        Project = new Project
                        {
                            Key = EXISTING_PROJECT
                        }
                    }
                },
                State = PullRequestState.OPEN
            };

            newPullRequest = await this.stashClient.PullRequests.Create(this.EXISTING_PROJECT, this.EXISTING_REPOSITORY, newPullRequest);

            Assert.IsTrue(newPullRequest.State != PullRequestState.MERGED);
            Assert.IsTrue(newPullRequest.Reviewers.Length == 0);

            newPullRequest.Reviewers = new[]
            {
                new AuthorWrapper
                {
                    User = new Author
                    {
                        Name = OTHERTEST_USERNAME
                    }
                }
            };

            newPullRequest = await this.stashClient.PullRequests.Update(newPullRequest, this.EXISTING_PROJECT, this.EXISTING_REPOSITORY);

            Assert.IsTrue(newPullRequest.Reviewers.Length == 1);

            this.stashClient.SetBasicAuthentication(this.OTHERTEST_USERNAME, this.OTHERTEST_PASSWORD);

            var pullRequestReviewed = await this.stashClient.PullRequests.Approve(newPullRequest, this.EXISTING_PROJECT);

            Assert.IsTrue(pullRequestReviewed.Approved);

            pullRequestReviewed = await this.stashClient.PullRequests.Decline(newPullRequest, this.EXISTING_PROJECT);

            Assert.IsFalse(pullRequestReviewed.Approved);

            this.stashClient.SetBasicAuthentication(this.TEST_USERNAME, this.TEST_PASSWORD);

            newPullRequest = await this.stashClient.PullRequests.Merge(newPullRequest, this.EXISTING_PROJECT);

            Assert.IsTrue(newPullRequest.State == PullRequestState.MERGED);
        }

        [TestMethod]
        public async Task Get_TestUser_User_With_BearAuthenticationToken()
        {
            var client = new StashClient(this.BASE_URL, this.PERSONAL_ACCESS_TOKEN, true);
            var response = await client.Users.Get(this.TEST_USERNAME);

            var users = response.Values;

            Assert.IsNotNull(users);
            Assert.IsTrue(users.Any());
            Assert.IsNotNull(users.Single(user => user.Name == this.TEST_USERNAME));
        }
    }
}
