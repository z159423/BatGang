#import <Foundation/Foundation.h>

@interface PersonalInfo : NSObject

+ (PersonalInfo *)sharedManager;
- (NSString *)getCountry;
- (NSString *)getSessionID;
- (NSString *)getApiKey;
- (NSString *)getAppID;
- (NSString *)getIDFA;
- (NSString *)getIDFV;
- (NSString *)getPlayOnID;
- (NSString *)getIABUSPrivacyString;
- (NSString *)getIsChildDirected;
- (BOOL)getGdprConsent;
- (void)setGdprConsent:(BOOL)flag;
- (void)setIABUSPrivacyString:(NSString *)consentString;
- (void)clearIABUSPrivacyString;
- (void)setIsChildDirected:(BOOL)isDirected;
- (void)setAppID:(NSString *)appID;
- (void)setApiKey:(NSString *)apiKey;
- (void)setSessionID:(NSString *)sessionID;
- (void)setCountry:(NSString *)country;
- (void)requestTrackingAuthorizationWithCompletionHandler:(void (^)(NSUInteger status))completion;
- (Class)appTrackingManager;
@end
