namespace Phoenix.Infrastructure {
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class ParseGif {
        private readonly List<int> _delays = new List<int>();

        public List<int> ParseGifDataStream(byte[] gifData, int offset) {
            _delays.Clear();
            offset = ParseHeader(ref gifData, offset);
            offset = ParseLogicalScreen(ref gifData, offset);
            while (offset != -1)
                offset = ParseBlock(ref gifData, offset);
            return _delays;
        }

        private static int ParseHeader(ref byte[] gifData, int offset) {
            var str = Encoding.ASCII.GetString(gifData, offset, 3);
            if (str != "GIF")
                throw new FormatException("Not a proper GIF file: missing GIF header");
            return 6;
        }

        private static int ParseLogicalScreen(ref byte[] gifData, int offset) {
            var packedField = gifData[offset + 4];
            var hasGlobalColorTable = (packedField & 0x80) > 0 ? true : false;

            var currentIndex = offset + 7;
            if (hasGlobalColorTable) {
                var colorTableLength = packedField & 0x07;
                colorTableLength = (int) Math.Pow(2, colorTableLength + 1)*3;
                currentIndex = currentIndex + colorTableLength;
            }
            return currentIndex;
        }

        private int ParseBlock(ref byte[] gifData, int offset) {
            switch (gifData[offset]) {
                case 0x21:
                    return gifData[offset + 1] == 0xF9
                               ? ParseGraphicControlExtension(ref gifData, offset)
                               : ParseExtensionBlock(ref gifData, offset);
                case 0x2C:
                    offset = ParseGraphicBlock(ref gifData, offset);
                    return offset;
                case 0x3B:
                    return -1;
                default:
                    throw new FormatException("GIF format incorrect: missing graphic block or special-purpose block. ");
            }
        }

        private int ParseGraphicControlExtension(ref byte[] gifData, int offset) {
            int length = gifData[offset + 2];
            var returnOffset = offset + length + 2 + 1;

            int delay = BitConverter.ToUInt16(gifData, offset + 4);
            var delayTime = (delay < 10) ? 10 : delay;
            _delays.Add(delayTime);
            while (gifData[returnOffset] != 0x00)
                returnOffset = returnOffset + gifData[returnOffset] + 1;

            returnOffset++;

            return returnOffset;
        }

        private static int ParseExtensionBlock(ref byte[] gifData, int offset) {
            int length = gifData[offset + 2];
            var returnOffset = offset + length + 2 + 1;
            while (gifData[returnOffset] != 0x00)
                returnOffset = returnOffset + gifData[returnOffset] + 1;

            returnOffset++;

            return returnOffset;
        }

        private static int ParseGraphicBlock(ref byte[] gifData, int offset) {
            var packedField = gifData[offset + 9];
            var hasLocalColorTable = (packedField & 0x80) > 0 ? true : false;

            var currentIndex = offset + 9;
            if (hasLocalColorTable) {
                var colorTableLength = packedField & 0x07;
                colorTableLength = (int) Math.Pow(2, colorTableLength + 1)*3;
                currentIndex = currentIndex + colorTableLength;
            }
            currentIndex++;
            currentIndex++;
            while (gifData[currentIndex] != 0x00) {
                currentIndex = currentIndex + gifData[currentIndex];
                currentIndex++;
            }
            currentIndex = currentIndex + 1;
            return currentIndex;
        }
    }
}