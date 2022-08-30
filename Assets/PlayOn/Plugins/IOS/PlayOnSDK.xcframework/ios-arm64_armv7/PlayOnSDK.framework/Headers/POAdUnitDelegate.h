@class POImpressionData;

@protocol POAdUnitDelegate
- (void)onAvailabilityChanged:(BOOL)flag;
- (void)onShow;
- (void)onClose;
- (void)onClick;
- (void)onReward:(float)amount;
@optional
- (void)onImpression:(POImpressionData *)impressionData;
@end

