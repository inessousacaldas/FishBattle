//
//  CropController.m
//  CropImage
//
//  Created by RYAN on 12-11-19.
//  Copyright (c) 2012年 杨 烽. All rights reserved.
//

#import "CropController.h"
#import "KICropImageView.h"
#import "UIImage+FixOrientation.h"


@implementation CropController
{
    KICropImageView *_cropImageView;
    UIImage *_image;
    CGSize _cropSize;
}

- (id)initWithImage:(UIImage *)image CropSize:(CGSize)size
{
    self = [super init];
    if (self) {
        _image = image;
#if !__has_feature(objc_arc)
        [_image retain];
#endif
        _cropSize = size;
    }
    return self;
}


#if !__has_feature(objc_arc)
- (void) dealloc
{
    [_image release];
    _image = nil;
    [_cropImageView release];
    _cropImageView = nil;
    
    [super dealloc];
}
#endif

- (id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil
{
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
        // Custom initialization
    }
    return self;
}

- (BOOL) prefersStatusBarHidden
{
    return YES;
}

- (void)viewDidLoad
{
    [super viewDidLoad];
    // Do any additional setup after loading the view from its nib.
    [self.navigationController setNavigationBarHidden:YES];
    
    CGRect frame = CGRectMake(0, 44, self.view.bounds.size.width, self.view.bounds.size.height);
    
    _cropImageView = [[KICropImageView alloc] initWithFrame:frame];
    [_cropImageView setCropSize:_cropSize];
    [_cropImageView setImage:_image];
    [self.view addSubview:_cropImageView];
    
    UINavigationBar *navBar = [[UINavigationBar alloc] initWithFrame:CGRectMake(0, 0, self.view.bounds.size.width, 44)];
    UINavigationItem *navItem = [[UINavigationItem alloc] initWithTitle:@"Move And Scale"];
    [navBar pushNavigationItem:navItem animated:YES];
    UIBarButtonItem *doneItem = [[UIBarButtonItem alloc] initWithTitle:@"Done" style:UIBarButtonSystemItemDone target:self action:@selector(doneBtnClick)];
    navItem.rightBarButtonItem = doneItem;
    UIBarButtonItem *cancelItem = [[UIBarButtonItem alloc] initWithTitle:@"Cancel" style:UIBarButtonSystemItemCancel target:self action:@selector(cancelBtnClick)];
    navItem.leftBarButtonItem = cancelItem;
    [self.view addSubview:navBar];
    
#if !__has_feature(objc_arc)
    [cancelItem release];
    [doneItem release];
    [navItem release];
    [navBar release];
#endif
    
//    [self doneBtnClick];
}

- (void) doneBtnClick
{
    [_delegate passImage:[_cropImageView cropImage]];
}

- (void) cancelBtnClick
{
    [_delegate passImage:nil];
}

- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

@end
