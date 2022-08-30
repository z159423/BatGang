#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import <PlayOnSDK/PlayOnManagerDelegate.h>
#import <PlayOnSDK/POLogs.h>
#import <PlayOnSDK/POAdUnit.h>

@interface PlayOnManager: NSObject
@property (nonatomic, weak) id<PlayOnManagerDelegate> delegate;

+ (PlayOnManager *)sharedManager;
- (BOOL)isInitialized;
- (void)initialize:(NSString *)appKey withStoreId:(NSString *)storeId;
- (void)setEngineInfo:(NSString *)engineName withVersion:(NSString *)engineVersion;
- (void)setGdprConsent:(BOOL)flag;
- (void)setLogLevel:(POLogLevel)level;
- (void)setIsChildDirected:(BOOL)flag;
- (void)setIABUSPrivacyString:(NSString *)consentString;
- (void)clearIABUSPrivacyString;
- (NSString *)getIsChildDirected;
- (NSString *)getSDKVersion;
- (NSString *)getDeviceVolumeLevel;
- (void)requestTrackingAuthorization;
+ (UIViewController *)unityViewController;
- (void)addCustomAttribute:(NSString *)key withValue:(NSString *)value;
- (NSArray *)getCustomAttributes;
- (NSArray *)getCustomAttributes:(NSString *)key;
- (void)clearCustomAttributes;
- (void)removeCustomAttributes:(NSString *)key;
- (void)addAdUnit:(POAdUnit *)unit;
- (void)removeAdUnit:(POAdUnit *)unit;
- (void)onPause;
- (void)onResume;
@end
