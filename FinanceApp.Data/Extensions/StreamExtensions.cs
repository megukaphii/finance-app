using System.Text;
using FinanceApp.Data.Exceptions;
using FinanceApp.Data.Utility;

namespace FinanceApp.Data.Extensions;

public static class StreamExtensions
{
	private const int ReadTimeout = 10000;

	public static async Task<string> ReadMessageAsync(this Stream stream)
	{
		byte[] buffer = new byte[2048];
		StringBuilder messageData = new();
		CancellationTokenSource source = new();
		bool readFirstBlock = false;
		do {
			if (readFirstBlock)
				source.CancelAfter(ReadTimeout);

			int bytes = await stream.ReadAsync(buffer, source.Token);
			readFirstBlock = true;

			if (bytes <= 0) throw new ConnectionException("Server connection timed out");

			messageData.Append(DecodeBuffer(buffer, bytes));
			if (messageData.ToString().Contains(Serialization.Eof)) {
				break;
			} else {
				source.Dispose();
				source = new();
			}
		} while (true);

		source.Dispose();
		return messageData.ToString().Replace(Serialization.Eof, "");
	}

	private static char[] DecodeBuffer(byte[] buffer, int bytes)
	{
		Decoder decoder = Encoding.UTF8.GetDecoder();
		char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
		decoder.GetChars(buffer, 0, bytes, chars, 0);
		return chars;
	}
}