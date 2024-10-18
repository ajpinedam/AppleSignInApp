#nullable enable

#if __IOS__
using AuthenticationServices;
using Foundation;
using UIKit;
#endif

namespace AppleSignInApp.Presentation;

public sealed partial class MainPage : Page
{
#if __IOS__
private AuthorizationControllerDelegate _appleSignInDelegate;

private AuthManager authManager;
#endif
    public MainPage()
    {
        this.InitializeComponent();
#if __IOS__
        var appleSignInButton = new ASAuthorizationAppleIdButton(ASAuthorizationAppleIdButtonType.Default, ASAuthorizationAppleIdButtonStyle.WhiteOutline);
        appleSignInButton.TouchUpInside += HandleAuthorizationAppleIDButtonPress;
        appleSignInButton.CornerRadius = 50;

        var adaptedAppleButton = VisualTreeHelper.AdaptNative(appleSignInButton);
        var borderWrapper = new Border
        {
            MinHeight = 50,
            MinWidth = 250,
            HorizontalAlignment = HorizontalAlignment.Center,
            Child = adaptedAppleButton,
        };

        m_MainStackPanel.Children.Add(borderWrapper);        
#endif
    }

#if __IOS__
   private void HandleAuthorizationAppleIDButtonPress(object? sender, EventArgs e)
   {
        authManager = new AuthManager(this.Window);
        //_appleSignInDelegate = new AuthorizationControllerDelegate(this);
       var appleIDProvider = new ASAuthorizationAppleIdProvider();

       var request = appleIDProvider.CreateRequest();
       request.RequestedScopes = new[] { ASAuthorizationScope.FullName, ASAuthorizationScope.Email };

       var authorizationController = new ASAuthorizationController(new[] { request });
       authorizationController.Delegate = authManager;  // Use the retained delegate
       authorizationController.PresentationContextProvider = authManager;  // Set the presentation context provider
       //authorizationController.PresentationContextProvider = new PresentationContextProvider();  // Set the presentation context provider
       authorizationController.PerformRequests();
   }
#endif   
}

#if __IOS__
public class AuthorizationControllerDelegate : ASAuthorizationControllerDelegate
{
    private readonly MainPage _parent;

    public AuthorizationControllerDelegate(MainPage parent)
    {
        _parent = parent;
    }

    public override void DidComplete(ASAuthorizationController controller, ASAuthorization authorization)
    {
        System.Diagnostics.Debug.WriteLine("Authorization successful.");

        try
        {
            var appleIdCredential = authorization.GetCredential<ASAuthorizationAppleIdCredential>();

            var userIdentifier = appleIdCredential?.User;
            // Handle successful authorization, retrieve user details
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Authorization failed: {ex.Message}");
        }
    }
}


class AuthManager : NSObject, IASAuthorizationControllerDelegate, IASAuthorizationControllerPresentationContextProviding
{
    public Task<ASAuthorizationAppleIdCredential> GetCredentialsAsync()
        => tcsCredential?.Task;

    TaskCompletionSource<ASAuthorizationAppleIdCredential> tcsCredential;

    UIWindow presentingAnchor;

    public AuthManager(UIWindow presentingWindow)
    {
        tcsCredential = new TaskCompletionSource<ASAuthorizationAppleIdCredential>();
        presentingAnchor = presentingWindow;
    }

    public UIWindow GetPresentationAnchor(ASAuthorizationController controller)
        => presentingAnchor;

    [Export("authorizationController:didCompleteWithAuthorization:")]
    public void DidComplete(ASAuthorizationController controller, ASAuthorization authorization)
    {
        var creds = authorization.GetCredential<ASAuthorizationAppleIdCredential>();
        tcsCredential?.TrySetResult(creds);
    }

    [Export("authorizationController:didCompleteWithError:")]
    public void DidComplete(ASAuthorizationController controller, NSError error)
        => tcsCredential?.TrySetException(new Exception(error.LocalizedDescription));
}


public partial class MainPage : IASAuthorizationControllerPresentationContextProviding
{
    public UIWindow GetPresentationAnchor(ASAuthorizationController controller)
    {
        return this.Window;
    }
}

public class PresentationContextProvider : NSObject, IASAuthorizationControllerPresentationContextProviding
{
    public UIWindow GetPresentationAnchor(ASAuthorizationController controller)
    {
        return UIApplication.SharedApplication.KeyWindow;
    }
}
#endif
