#import "UnityAppController.h"
#include "AudioPluginInterface.h"

@interface UnityHelmAppController : UnityAppController {}
@end

@implementation UnityHelmAppController

- (void)preStartUnity {
    [super preStartUnity];
    UnityRegisterAudioPlugin(&UnityGetAudioEffectDefinitions);
}

@end

IMPL_APP_CONTROLLER_SUBCLASS(UnityHelmAppController)
