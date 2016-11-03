//
//  RateMe.h
//  Unity-iPhone
//
//  Created by Eugenio on 02/18/15.
//
//

FOUNDATION_EXPORT NSString *IAPPrefix();
FOUNDATION_EXPORT const char *IAPPrefixForUnity();


FOUNDATION_EXTERN const char *AdjustAppToken();
FOUNDATION_EXTERN const bool AdjustEnvironmentIsProduction();

FOUNDATION_EXTERN NSString *TapJoyIOSKey();
FOUNDATION_EXTERN const bool TapJoyDebug();

FOUNDATION_EXTERN const char * FyberAppId();
FOUNDATION_EXTERN const char * FyberUserId();
FOUNDATION_EXTERN const char * FyberSecurityToken();
FOUNDATION_EXTERN const bool FyberDebug();

FOUNDATION_EXTERN const char * HeyZapAppId();
FOUNDATION_EXTERN const bool HeyZapDebug();

FOUNDATION_EXTERN const char * ITunesLink();
