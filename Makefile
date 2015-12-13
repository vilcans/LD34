PROJECT=ld34
VERSION=0.1.0-$(shell date '+%Y%m%d.%H%M%S')
#VERSION=1.0.0-2
FILENAME=$(PROJECT)-$(VERSION)
RELEASE_DIR=Build/release

release: release-win release-mac release-linux

release-win: $(RELEASE_DIR)/$(FILENAME)-win32.zip

$(RELEASE_DIR)/$(FILENAME)-win32.zip:
	mkdir -p Build/release
	rm -rf Build/ziptemp
	mkdir -p Build/ziptemp
	cp -r Build/win32 Build/ziptemp/$(FILENAME)
	bash -c 'cd Build/ziptemp && zip -x .DS_Store -r ../release/$(FILENAME)-win32.zip $(FILENAME)'

release-mac: $(RELEASE_DIR)/$(FILENAME).dmg

$(RELEASE_DIR)/$(FILENAME).dmg:
	mkdir -p Build/release
	hdiutil create $@ -volname "$(PROJECT)" -srcfolder Build/mac

release-linux: $(RELEASE_DIR)/$(FILENAME)-linux.tar.gz

$(RELEASE_DIR)/$(FILENAME)-linux.tar.gz:
	mkdir -p Build/release
	rm -rf Build/ziptemp
	mkdir -p Build/ziptemp
	cp -r Build/linux Build/ziptemp/$(FILENAME)
	tar -czf $@ --exclude '.DS_Store' -C Build/ziptemp $(FILENAME)

clean:
	rm -rf Build/release
	rm -rf Build/ziptemp

%.gif:%.mov
	ffmpeg -i $< -vf scale=320:-1 -pix_fmt rgb24 -r 10 -f gif - | gifsicle --optimize=3 --delay=9 >$@

.PHONY: release
