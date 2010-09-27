using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace Kiss.Utils.Imaging
{
	public class AnimatedGifDecoder
	{
		protected Image _image;
		protected List<Bitmap> _nodes;
		protected System.Drawing.Imaging.FrameDimension _ImageDimension;
		protected bool _canPlay;
		protected int _frameCount;
		protected int _currenTime;
		protected ulong _timeSum;
		protected int _frameFrequency;
		protected int _allFrameTime;
		protected Size _size;
		protected byte[] _time;
		protected int _cycleCount;
		protected int _currentCycleCount;

		/// <summary>
		/// 获取或者设置当前的Image对象
		/// </summary>
		public Image Image
		{
			get
			{
				return _image;
			}
			set
			{
				_image = value;
			}
		}
		/// <summary>
		/// 当前的图片
		/// </summary>
		public Image CurrentImage
		{
			get
			{
				return _image;
			}
			set
			{
				_image = value;
			}
		}

		/// <summary>
		/// 当前的Image需要播放的循环数
		/// </summary>
		public int CycleCount
		{
			get
			{
				return _cycleCount;
			}
			set
			{
				_cycleCount = value;
			}
		}
		/// <summary>
		/// 当前是否可以播放这个,有可能是一个静态图片，也有可能是一个动态图片，但是现在不允许播放了
		/// </summary>
		public bool CanPlay
		{
			get
			{
				if (!_canPlay)
					return false;

				if (_cycleCount == 0 || _currentCycleCount < _cycleCount)
				{
					return true;
				}
				if (_currentCycleCount >= _cycleCount)
				{
					_timeSum = 0;
					return false;
				}
				return _canPlay;
			}
		}
		/// <summary>
		/// 开始播放动画，不改变动画原先所在的位置
		/// </summary>
		public void Start()
		{
			_currentCycleCount = 0;
			if (_frameCount > 1)
				_canPlay = true;
		}

		/// <summary>
		/// 立即停止,动画停止在当前的位置
		/// </summary>
		public void Stop()
		{
			if (_frameCount > 1)
				_canPlay = false;
			_timeSum = 0;
		}

		/// <summary>
		/// 当前这个图片的桢
		/// </summary>
		public int FrameCount
		{
			get
			{
				return _frameCount;
			}

		}


		/// <summary>
		/// 是否是合法的图片
		/// </summary>
		/// <returns></returns>
		public bool IsValidImage()
		{
			if (_image != null)
				return true;
			else
				return false;
		}

		/// <summary>
		/// 当前的图片是否是动画图片
		/// </summary>
		public bool IsAnimated
		{
			get
			{
				if (_frameCount > 1)
					return true;
				return false;
			}
		}
		/// <summary>
		/// 从图片文件中获取数据
		/// </summary>
		/// <param name="FilePath"></param>
		public void FromFile(string FilePath)
		{
			try // Check If invalid horision 2006-3-23
			{
				_image = Image.FromFile(FilePath);
			}
			catch
			{
				_image = null;
				return;
			}

			ParseImage();

		}
		/// <summary>
		/// 解析Image对象
		/// </summary>
		private void ParseImage()
		{
			if (_image.FrameDimensionsList.GetLength(0) != 0)
			{
				Debug.Assert(_image.FrameDimensionsList.GetLength(0) != 0);
			}

			_ImageDimension = new System.Drawing.Imaging.FrameDimension(_image.FrameDimensionsList[0]);
			_frameCount = _image.GetFrameCount(_ImageDimension);
			if (FrameCount > 1)
			{
				_canPlay = true;
				_frameFrequency = _image.PropertyItems[0].Value.GetLength(0) / FrameCount;
				_time = _image.PropertyItems[0].Value;
			}
			else
			{
				_canPlay = false;

			}

			for (int i = 0; i < FrameCount; i++)
			{
				_allFrameTime = _allFrameTime + GetFrameTime(i);
			}
			_currenTime = 0;
			_size = _image.Size;
			for (int i = 0; i < FrameCount; i++)
			{
				MemoryStream s = new MemoryStream();
				_image.SelectActiveFrame(_ImageDimension, i);

				Bitmap bitmap = new Bitmap(_image);
				_nodes.Add(bitmap);
				s.Dispose();
			}
		}

		/// <summary>
		/// 根据传送的Image 对象构造AmigoImage
		/// </summary>
		/// <param name="image"></param>
		public AnimatedGifDecoder(Image image)
		{
			ClassInitCollection();
			_image = image;
			if (_image == null)
			{
				return;
			}
			ParseImage();

		}

		/// <summary>
		/// AmigoImage 的构造函数
		/// </summary>
		public AnimatedGifDecoder()
		{
			ClassInitCollection();
		}
		/// <summary>
		/// AmigoImage的构造函数，可以直接根据文件获取实例
		/// </summary>
		/// <param name="FilePath"></param>
		public AnimatedGifDecoder(string FilePath)
		{
			ClassInitCollection();
			FromFile(FilePath);
		}


		private void ClassInitCollection()
		{
			_nodes = new List<Bitmap>();
			_frameFrequency = 0;

		}
		/// <summary>
		/// 获得指定桢的事件长度
		/// </summary>
		/// <param name="frame"></param>
		/// <returns></returns>
		protected int GetFrameTime(int frame)
		{
			Debug.Assert(frame <= FrameCount);
			if (frame > FrameCount)
			{
				return -1;
			}
			if (FrameCount == 1)
				return 0;
			byte[] Time = _time;

			int iTime = Time[frame * _frameFrequency + 3] * (int)Math.Pow(10, 4) +
				Time[frame * _frameFrequency + 2] * (int)Math.Pow(10, 3) +
				Time[frame * _frameFrequency + 1] * (int)Math.Pow(10, 2) +
				Time[frame * _frameFrequency + 0] * (int)Math.Pow(10, 1);
			return iTime;

		}

		/// <summary>
		/// 图片的Size 长*宽
		/// </summary>
		public Size Size
		{
			get
			{
				return _size;
			}
		}
		/// <summary>
		/// 图片的高度
		/// </summary>
		public int Height
		{
			get
			{
				return _size.Height;
			}
		}
		/// <summary>
		/// 图片的宽度
		/// </summary>
		public int Width
		{
			get
			{
				return _size.Width;
			}
		}

		/// <summary>
		/// 获得下一个事件段的图片，并将CurrentImage 指到当前时间片应该定位到的图片
		/// </summary>
		/// <param name="interval"></param>
		/// <returns></returns>
		public virtual Image NextFrame(int interval)
		{

			if (!CanPlay) return _image;
			_timeSum += (ulong)interval;
			_currentCycleCount = (int)(_timeSum / (ulong)_allFrameTime);
			_currenTime = (_currenTime + interval) % _allFrameTime;


			int iTemptime = 0;
			int frame = 0;
			for (int i = 0; i < FrameCount; i++)
			{
				iTemptime = iTemptime + GetFrameTime(i);
				if (iTemptime >= _currenTime)
				{
					frame = i;
					break;
				}
			}
			// _image.SelectActiveFrame(_ImageDimension, frame);
			_image = _nodes[frame];
			return _image;
		}

		public GifFrame GetFrame(int frame)
		{
			Image frameImage = _nodes[frame];
			int delay = GetFrameTime(frame);
			return new GifFrame(frameImage, delay);
		}
	}
}
