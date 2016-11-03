//
//  LLAppController.m
//  LLLibSet
//
//  Created by anGGod on 5/6/15.
//  Copyright (c) 2015 My Company. All rights reserved.
//

#import "LLAppController.h"
#import <Fabric/Fabric.h>
#import <Crashlytics/Crashlytics.h>


@implementation LLAppController

- (void)application:(UIApplication *)application preDidFinishLaunchingWithOptions:(NSDictionary *)launchOptions;
{
    // Crashlytics
    
    NSDictionary *fabricDict = [[NSBundle mainBundle] objectForInfoDictionaryKey:@"Fabric"];
    
    if (fabricDict != nil)
    {
        if ([fabricDict objectForKey:@"APIKey"] != nil)
        {
           [Fabric with:@[CrashlyticsKit]];
        }
    }
}


@end


IMPL_APP_CONTROLLER_SUBCLASS(LLAppController)
