//
//  KICropImageView.m
//  Kitalker
//
//  Created by 杨 烽 on 12-8-9.
//
//

#import "KICropImageView.h"
#import "UIImage+KIAdditions.h"


@class KICropImageMaskView;
@implementation KICropImageView

- (void)setFrame:(CGRect)frame {
    [super setFrame:frame];
    [[self scrollView] setFrame:self.bounds];
    [[self maskView] setFrame:self.bounds];
    
    _useViewScale = 0.8;
    
    if (CGSizeEqualToSize(_cropSize, CGSizeZero)) {
        [self setCropSize:CGSizeMake(100, 100)];
    }
}

- (UIScrollView *)scrollView {
    if (_scrollView == nil) {
        _scrollView = [[UIScrollView alloc] init];
        [_scrollView setDelegate:self];
        [_scrollView setBounces:NO];
        [_scrollView setShowsHorizontalScrollIndicator:NO];
        [_scrollView setShowsVerticalScrollIndicator:NO];
        [self addSubview:_scrollView];
    }
    return _scrollView;
}

- (UIImageView *)imageView {
    if (_imageView == nil) {
        _imageView = [[UIImageView alloc] init];
        [[self scrollView] addSubview:_imageView];
    }
    return _imageView;
}

- (KICropImageMaskView *)MaskView {
    if (_maskView == nil) {
        _maskView = [[KICropImageMaskView alloc] init];
        [_maskView setBackgroundColor:[UIColor clearColor]];
        [_maskView setUserInteractionEnabled:NO];
        [self addSubview:_maskView];
        [self bringSubviewToFront:_maskView];
    }
    return _maskView;
}

- (void)setImage:(UIImage *)image {
    if (image != _image) {
#if !__has_feature(objc_arc)
        [_image release];
        _image = nil;
        _image = [image retain];
#else
        _image = image;
#endif
    }
    [[self imageView] setImage:_image];
    
    [self updateZoomScale];

    [[self scrollView] setContentOffset:CGPointMake(([self scrollView].contentSize.width -CGRectGetWidth(self.bounds)) * 0.5, ([self scrollView].contentSize.height - CGRectGetHeight(self.bounds)) * 0.5)];
}

- (void)updateZoomScale {
    CGFloat width = _image.size.width;
    CGFloat height = _image.size.height;
    
    [[self imageView] setFrame:CGRectMake(0, 0, width, height)];
    
    CGFloat xScale = _cropSize.width / width;
    CGFloat yScale = _cropSize.height / height;
    
    CGFloat min = MAX(xScale, yScale);
    CGFloat max = 1.0;
    
    /*
    if ([[UIScreen mainScreen] respondsToSelector:@selector(scale)]) {
        max = 1.0 / [[UIScreen mainScreen] scale];
    } */
    
    if (min > max) {
        min = max;
    }
    
    [[self scrollView] setMinimumZoomScale:min];
    [[self scrollView] setMaximumZoomScale:max + 5.0f];
    
    [[self scrollView] setZoomScale:1 animated:YES];
}

- (void)setCropSize:(CGSize)size {
    
    if (size.width / CGRectGetWidth(self.bounds) > size.height / CGRectGetHeight(self.bounds))
    {
        _cropScale = CGRectGetWidth(self.bounds) / size.width;
    }
    else
    {
        _cropScale = CGRectGetHeight(self.bounds) / size.height;
    }
    _cropScale = _cropScale * _useViewScale;
    _cropSize = CGSizeMake(size.width * _cropScale, size.height * _cropScale);
//    _cropSize = size;
    [self updateZoomScale];
    
    CGFloat width = _cropSize.width;
    CGFloat height = _cropSize.height;
    
    CGFloat x = (CGRectGetWidth(self.bounds) - width) / 2;
    CGFloat y = (CGRectGetHeight(self.bounds) - height) / 2;

    [[self MaskView] setCropSize:_cropSize];
    
    CGFloat top = y;
    CGFloat left = x;
    CGFloat right = CGRectGetWidth(self.bounds)- width - x;
    CGFloat bottom = CGRectGetHeight(self.bounds)- height - y;
    _imageInset = UIEdgeInsetsMake(top, left, bottom, right);
    [[self scrollView] setContentInset:_imageInset];
    
    [[self scrollView] setContentOffset:CGPointMake(([self scrollView].contentSize.width -CGRectGetWidth(self.bounds)) * 0.5, ([self scrollView].contentSize.height - CGRectGetHeight(self.bounds)) * 0.5)];
//    [[self scrollView] setContentOffset:CGPointMake(0, 0)];
}

- (UIImage *)cropImage {
    CGFloat zoomScale = [self scrollView].zoomScale;
    
    CGFloat offsetX = [self scrollView].contentOffset.x;
    CGFloat offsetY = [self scrollView].contentOffset.y;
    CGFloat aX = offsetX>=0 ? offsetX+_imageInset.left : (_imageInset.left - ABS(offsetX));
    CGFloat aY = offsetY>=0 ? offsetY+_imageInset.top : (_imageInset.top - ABS(offsetY));
    
    aX = aX / zoomScale;
    aY = aY / zoomScale;
    
//    CGFloat aWidth =  MAX(_cropSize.width / zoomScale, _cropSize.width);
//    CGFloat aHeight = MAX(_cropSize.height / zoomScale, _cropSize.height);
    CGFloat aWidth =  _cropSize.width / zoomScale;
    CGFloat aHeight = _cropSize.height / zoomScale;
    
#ifdef DEBUG
    NSLog(@"%f--%f--%f--%f", aX, aY, aWidth, aHeight);
#endif
    
    UIImage *image = [_image cropImageWithX:aX y:aY width:aWidth height:aHeight];
    image = [image resizeToWidth:_cropSize.width / _cropScale height:_cropSize.height / _cropScale];
    
    //    CGRect rect = CGRectMake(aX, aY, aWidth, aHeight);
    //    CGImageRef imageRef = _image.CGImage;
    //    CGImageRef subImageRef = CGImageCreateWithImageInRect(imageRef, rect);
    //    UIGraphicsBeginImageContext(_cropSize);
    //    CGContextRef context = UIGraphicsGetCurrentContext();
    //    CGContextDrawImage(context, CGRectMake(0, 0, _cropSize.width, _cropSize.height), subImageRef);
    //    UIImage *image = UIGraphicsGetImageFromCurrentImageContext();
    //    UIGraphicsEndImageContext();
    
    return image;
}

#pragma UIScrollViewDelegate
- (UIView *)viewForZoomingInScrollView:(UIScrollView *)scrollView {
    return [self imageView];
}

#if !__has_feature(objc_arc)
- (void)dealloc {
    [_scrollView release];
    _scrollView = nil;
    [_imageView release];
    _imageView = nil;
    [_maskView release];
    _maskView = nil;
    [_image release];
    _image = nil;
    [super dealloc];
}
#endif
@end

#pragma KISnipImageMaskView

#define kMaskViewBorderWidth 1.0f

@implementation KICropImageMaskView

- (void)setCropSize:(CGSize)size {
    CGFloat x = (CGRectGetWidth(self.bounds) - size.width) / 2;
    CGFloat y = (CGRectGetHeight(self.bounds) - size.height) / 2;
    _cropRect = CGRectMake(x, y, size.width, size.height);
    
    [self setNeedsDisplay];
}

- (CGSize)cropSize {
    return _cropRect.size;
}

- (void)drawRect:(CGRect)rect {
    [super drawRect:rect];
    CGContextRef ctx = UIGraphicsGetCurrentContext();
    CGContextSetRGBFillColor(ctx, 0, 0, 0, .4);
    CGContextFillRect(ctx, self.bounds);
    
    CGContextSetStrokeColorWithColor(ctx, [UIColor whiteColor].CGColor);
    CGContextStrokeRectWithWidth(ctx, _cropRect, kMaskViewBorderWidth);
    
    CGContextClearRect(ctx, _cropRect);
}
@end
