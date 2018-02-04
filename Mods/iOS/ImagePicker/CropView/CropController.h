//
//  CropController.h
//  CropImage
//
//  Created by RYAN on 12-11-19.
//  Copyright (c) 2012年 杨 烽. All rights reserved.
//

#import <UIKit/UIKit.h>


@protocol PassImageDelegate <NSObject>

- (void) passImage:(UIImage *)image;

@end

@interface CropController : UIViewController

- (id)initWithImage:(UIImage *)image CropSize:(CGSize)size;

- (BOOL)prefersStatusBarHidden NS_AVAILABLE_IOS(7_0);

@property(assign) NSObject<PassImageDelegate> *delegate;

@end
