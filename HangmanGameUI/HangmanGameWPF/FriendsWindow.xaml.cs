using System;
using System.Windows;
using System.Windows.Input;
using HangmanGameWPF.Localization;
using HangmanGameWPF.Services;

namespace HangmanGameWPF
{
    public partial class FriendsWindow : Window
    {
        public FriendsWindow()
        {
            InitializeComponent();
            LoadAllData();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            => DragMove();

        private void BtnClose_Click(object sender, RoutedEventArgs e)
            => Close();

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        private int GetCurrentUserId()
        {
            return SessionManager.UserId;
        }

        private void LoadAllData()
        {
            LoadFriends();
            LoadPendingRequests();
            LoadSentRequests();
        }

        private void LoadFriends()
        {
            IFriendService client = null;

            try
            {
                client = ServiceClientFactory.CreateFriendClient();

                FriendOperationResultDto result = client.GetFriends(GetCurrentUserId());

                if (result != null && result.Success)
                {
                    LstFriends.ItemsSource = result.Friends;
                    SetStatus(result.Message);
                }
                else
                {
                    LstFriends.ItemsSource = null;
                    SetStatus(result?.Message ?? ClientLocalizer.Get("ERROR_FRIEND_LOAD"));
                }
            }
            catch (Exception ex)
            {
                SetStatus(string.Format(ClientLocalizer.Get("ERROR_FRIEND_LOAD_EXCEPTION"), ex.Message));
            }
            finally
            {
                ServiceClientFactory.CloseChannel(client);
            }
        }

        private void LoadPendingRequests()
        {
            IFriendService client = null;

            try
            {
                client = ServiceClientFactory.CreateFriendClient();

                FriendOperationResultDto result = client.GetPendingFriendRequests(GetCurrentUserId());

                if (result != null && result.Success)
                {
                    LstPendingRequests.ItemsSource = result.FriendRequests;
                }
                else
                {
                    LstPendingRequests.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                SetStatus(string.Format(ClientLocalizer.Get("ERROR_LOAD_PENDING_REQUESTS"), ex.Message));
            }
            finally
            {
                ServiceClientFactory.CloseChannel(client);
            }
        }

        private void LoadSentRequests()
        {
            IFriendService client = null;

            try
            {
                client = ServiceClientFactory.CreateFriendClient();

                FriendOperationResultDto result = client.GetSentFriendRequests(GetCurrentUserId());

                if (result != null && result.Success)
                {
                    LstSentRequests.ItemsSource = result.FriendRequests;
                }
                else
                {
                    LstSentRequests.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                SetStatus(string.Format(ClientLocalizer.Get("ERROR_LOAD_SENT_REQUESTS"), ex.Message));
            }
            finally
            {
                ServiceClientFactory.CloseChannel(client);
            }
        }

        private void BtnRefreshFriends_Click(object sender, RoutedEventArgs e)
        {
            LoadAllData();
        }

        private void BtnRemoveFriend_Click(object sender, RoutedEventArgs e)
        {
            FriendDto selectedFriend = LstFriends.SelectedItem as FriendDto;

            if (selectedFriend == null)
            {
                SetStatus(ClientLocalizer.Get("ERROR_SELECT_FRIEND"));
                return;
            }

            MessageBoxResult confirmation = MessageBox.Show(
                string.Format(ClientLocalizer.Get("REMOVE_FRIEND_CONFIRM"), selectedFriend.FullName),
                ClientLocalizer.Get("REMOVE_FRIEND_TITLE"),
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (confirmation != MessageBoxResult.Yes)
            {
                return;
            }

            IFriendService client = null;

            try
            {
                client = ServiceClientFactory.CreateFriendClient();

                RemoveFriendDto requestDto = new RemoveFriendDto
                {
                    CurrentUserId = GetCurrentUserId(),
                    FriendUserId = selectedFriend.UserId
                };

                FriendOperationResultDto result = client.RemoveFriend(requestDto);

                SetStatus(result?.Message ?? ClientLocalizer.Get("ERROR_SERVER_EMPTY"));

                if (result != null && result.Success)
                {
                    LoadAllData();
                }
            }
            catch (Exception ex)
            {
                SetStatus(string.Format(ClientLocalizer.Get("ERROR_REMOVE_FRIEND"), ex.Message));
            }
            finally
            {
                ServiceClientFactory.CloseChannel(client);
            }
        }

        private void BtnSendRequest_Click(object sender, RoutedEventArgs e)
        {
            string receiverEmail = TxtReceiverEmail.Text.Trim();

            if (string.IsNullOrWhiteSpace(receiverEmail))
            {
                SetStatus(ClientLocalizer.Get("ERROR_FRIEND_EMAIL_REQUIRED"));
                return;
            }

            IFriendService client = null;

            try
            {
                client = ServiceClientFactory.CreateFriendClient();

                SendFriendRequestByEmailDto requestDto = new SendFriendRequestByEmailDto
                {
                    SenderUserId = GetCurrentUserId(),
                    ReceiverEmail = receiverEmail
                };

                FriendOperationResultDto result = client.SendFriendRequestByEmail(requestDto);

                SetStatus(result?.Message ?? ClientLocalizer.Get("ERROR_SERVER_EMPTY"));

                if (result != null && result.Success)
                {
                    TxtReceiverEmail.Clear();
                    LoadAllData();
                }
            }
            catch (Exception ex)
            {
                SetStatus(string.Format(ClientLocalizer.Get("ERROR_SEND_FRIEND_REQUEST"), ex.Message));
            }
            finally
            {
                ServiceClientFactory.CloseChannel(client);
            }
        }

        private void BtnAcceptRequest_Click(object sender, RoutedEventArgs e)
        {
            FriendRequestDto selectedRequest = LstPendingRequests.SelectedItem as FriendRequestDto;

            if (selectedRequest == null)
            {
                SetStatus(ClientLocalizer.Get("ERROR_SELECT_PENDING_REQUEST"));
                return;
            }

            RespondToRequest(
                selectedRequest.FriendRequestId,
                accept: true,
                cancel: false
            );
        }

        private void BtnRejectRequest_Click(object sender, RoutedEventArgs e)
        {
            FriendRequestDto selectedRequest = LstPendingRequests.SelectedItem as FriendRequestDto;

            if (selectedRequest == null)
            {
                SetStatus(ClientLocalizer.Get("ERROR_SELECT_PENDING_REQUEST"));
                return;
            }

            RespondToRequest(
                selectedRequest.FriendRequestId,
                accept: false,
                cancel: false
            );
        }

        private void BtnCancelRequest_Click(object sender, RoutedEventArgs e)
        {
            FriendRequestDto selectedRequest = LstSentRequests.SelectedItem as FriendRequestDto;

            if (selectedRequest == null)
            {
                SetStatus(ClientLocalizer.Get("ERROR_SELECT_SENT_REQUEST"));
                return;
            }

            RespondToRequest(
                selectedRequest.FriendRequestId,
                accept: false,
                cancel: true
            );
        }

        private void RespondToRequest(int friendRequestId, bool accept, bool cancel)
        {
            IFriendService client = null;

            try
            {
                client = ServiceClientFactory.CreateFriendClient();

                RespondFriendRequestDto requestDto = new RespondFriendRequestDto
                {
                    FriendRequestId = friendRequestId,
                    CurrentUserId = GetCurrentUserId()
                };

                FriendOperationResultDto result;

                if (cancel)
                {
                    result = client.CancelFriendRequest(requestDto);
                }
                else if (accept)
                {
                    result = client.AcceptFriendRequest(requestDto);
                }
                else
                {
                    result = client.RejectFriendRequest(requestDto);
                }

                SetStatus(result?.Message ?? ClientLocalizer.Get("ERROR_SERVER_EMPTY"));

                if (result != null && result.Success)
                {
                    LoadAllData();
                }
            }
            catch (Exception ex)
            {
                SetStatus(string.Format(ClientLocalizer.Get("ERROR_PROCESSING_REQUEST"), ex.Message));
            }
            finally
            {
                ServiceClientFactory.CloseChannel(client);
            }
        }

        private void SetStatus(string message)
        {
            TxtStatus.Text = message;
        }
    }
}
