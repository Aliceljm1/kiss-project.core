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
		/// ��ȡ�������õ�ǰ��Image����
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
		/// ��ǰ��ͼƬ
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
		/// ��ǰ��Image��Ҫ���ŵ�ѭ����
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
		/// ��ǰ�Ƿ���Բ������,�п�����һ����̬ͼƬ��Ҳ�п�����һ����̬ͼƬ���������ڲ���������
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
		/// ��ʼ���Ŷ��������ı䶯��ԭ�����ڵ�λ��
		/// </summary>
		public void Start()
		{
			_currentCycleCount = 0;
			if (_frameCount > 1)
				_canPlay = true;
		}

		/// <summary>
		/// ����ֹͣ,����ֹͣ�ڵ�ǰ��λ��
		/// </summary>
		public void Stop()
		{
			if (_frameCount > 1)
				_canPlay = false;
			_timeSum = 0;
		}

		/// <summary>
		/// ��ǰ���ͼƬ����
		/// </summary>
		public int FrameCount
		{
			get
			{
				return _frameCount;
			}

		}


		/// <summary>
		/// �Ƿ��ǺϷ���ͼƬ
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
		/// ��ǰ��ͼƬ�Ƿ��Ƕ���ͼƬ
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
		/// ��ͼƬ�ļ��л�ȡ����
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
		/// ����Image����
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
		/// ���ݴ��͵�Image ������AmigoImage
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
		/// AmigoImage �Ĺ��캯��
		/// </summary>
		public AnimatedGifDecoder()
		{
			ClassInitCollection();
		}
		/// <summary>
		/// AmigoImage�Ĺ��캯��������ֱ�Ӹ����ļ���ȡʵ��
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
		/// ���ָ������¼�����
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
		/// ͼƬ��Size ��*��
		/// </summary>
		public Size Size
		{
			get
			{
				return _size;
			}
		}
		/// <summary>
		/// ͼƬ�ĸ߶�
		/// </summary>
		public int Height
		{
			get
			{
				return _size.Height;
			}
		}
		/// <summary>
		/// ͼƬ�Ŀ��
		/// </summary>
		public int Width
		{
			get
			{
				return _size.Width;
			}
		}

		/// <summary>
		/// �����һ���¼��ε�ͼƬ������CurrentImage ָ����ǰʱ��ƬӦ�ö�λ����ͼƬ
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
